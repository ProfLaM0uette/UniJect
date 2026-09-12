using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class DIContainer : IResolver, IInstantiator, IServiceProvider, IDisposable
    {
        #region Statements

        private static readonly object[] NO_ARGUMENTS = new object[0];

        private readonly Registration[] _registrations;
        private readonly Dictionary<ServiceIdentifier, RegistrationGroup> _index;
        private readonly InstanceStore _store;
        private readonly DisposalTracker _disposal;
        private readonly CallSiteFactory _callSites;
        private readonly CallSiteChain _chain;
        private readonly PendingInjectionSet _pending;
        private readonly LifecycleRunner _lifecycle;
        private readonly List<DIContainer> _children;
        private readonly IInjector _injector;
        private readonly IInstanceLivenessPolicy _liveness;
        private readonly IResolutionObserver _observer;
        private readonly ContainerOptions _options;

        public DIContainer Root { get; }
        public DIContainer Parent { get; }
        public bool IsDisposed { get; private set; }

        internal IInjector Injector
        {
            get { return _injector; }
        }

        internal IReadOnlyList<Registration> Registrations
        {
            get { return _registrations; }
        }

        internal ContainerOptions Options
        {
            get { return _options; }
        }

        internal CallSiteFactory CallSites
        {
            get { return _callSites; }
        }

        internal int SlotCount { get; }

        internal DIContainer(
            DIContainer parent,
            Registration[] registrations,
            Dictionary<ServiceIdentifier, RegistrationGroup> index,
            int storeSize,
            ContainerOptions options)
        {
            Parent = parent;
            Root = parent == null ? this : parent.Root;

            _registrations = registrations;
            _index = index;
            _options = options;
            _injector = options.ResolveInjector();
            _liveness = options.ResolveLiveness();
            _observer = options.ResolveObserver();
            SlotCount = storeSize;
            _store = new InstanceStore(storeSize);
            _disposal = new DisposalTracker(options.ResolveReleasers());
            _callSites = new CallSiteFactory(this);
            _chain = parent == null ? new CallSiteChain() : parent._chain;
            _pending = new PendingInjectionSet();
            _lifecycle = new LifecycleRunner();
            _children = new List<DIContainer>();

            parent?._children.Add(this);
        }

        #endregion

        #region Methods

        public object Resolve(Type contractType)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType));

            ResolutionRequest request = ResolutionRequest.ForRoot(contractType, null);
            return ResolveRequest(in request, null, 0);
        }

        public object ResolveId(Type contractType, object id)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType));

            ResolutionRequest request = ResolutionRequest.ForRoot(contractType, id);
            return ResolveRequest(in request, null, 0);
        }

        public bool TryResolve(Type contractType, out object instance)
        {
            return TryResolveId(contractType, null, out instance);
        }

        public bool TryResolveId(Type contractType, object id, out object instance)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType));

            ResolutionRequest request = ResolutionRequest.ForRoot(contractType, id);
            return TryResolveRequest(in request, null, 0, out instance);
        }

        public IReadOnlyList<object> ResolveAll(Type contractType)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType));

            AssertUsable("resolve");
            MainThreadGuard.Assert();

            ResolutionRequest request = ResolutionRequest.ForRoot(contractType, null);
            return ResolveElements(contractType, in request);
        }

        public ValidationReport Validate()
        {
            AssertUsable("validate");
            return ContainerValidator.Validate(this);
        }

        public bool HasBinding(Type contractType, object id)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType));

            ServiceIdentifier identifier = new ServiceIdentifier(contractType, id);

            for (DIContainer current = this; current != null; current = current.Parent)
            {
                if (current._index.ContainsKey(identifier))
                    return true;
            }

            return false;
        }

        public object CreateInstance(Type concreteType)
        {
            return CreateInstance(concreteType, NO_ARGUMENTS);
        }

        public object CreateInstance(Type concreteType, IReadOnlyList<object> arguments)
        {
            if (concreteType == null)
                throw new ArgumentNullException(nameof(concreteType));

            AssertUsable("create an instance");
            MainThreadGuard.Assert();

            ResolutionRequest request = ResolutionRequest.ForRoot(concreteType, null);
            ResolutionContext context = new ResolutionContext(this, in request, arguments, 0, _chain.Path);

            object created = _injector.CreateInstance(concreteType, in context);

            if (_pending.TryBegin(created))
                _injector.Inject(created, in context);

            return created;
        }

        public void Inject(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            AssertUsable("inject");
            MainThreadGuard.Assert();

            if (!_pending.TryBegin(target))
                return;

            ResolutionRequest request = ResolutionRequest.ForRoot(target.GetType(), null);
            ResolutionContext context = new ResolutionContext(this, in request, NO_ARGUMENTS, 0, _chain.Path);

            try
            {
                _injector.Inject(target, in context);
            }
            catch
            {
                _pending.Forget(target);
                throw;
            }

            _observer.InjectionCompleted(target, null);
        }

        public object GetService(Type serviceType)
        {
            return TryResolve(serviceType, out object instance) ? instance : null;
        }

        public IContainerBuilder CreateChildBuilder()
        {
            AssertUsable("create a child scope");
            return new ContainerBuilder(this);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            for (int i = _children.Count - 1; i >= 0; i--)
                _children[i].Dispose();

            _children.Clear();
            Parent?._children.Remove(this);

            try
            {
                _disposal.DisposeAll();
            }
            finally
            {
                _store.Clear();
                _callSites.Clear();
                _lifecycle.Clear();
            }
        }


        internal bool TryGetGroup(in ServiceIdentifier identifier, out RegistrationGroup group)
        {
            return _index.TryGetValue(identifier, out group);
        }

        internal void AdoptToRoot(object instance)
        {
            if (ReferenceEquals(this, Root))
                return;

            for (DIContainer current = this; current != null && current != Root; current = current.Parent)
            {
                if (!current._disposal.TryDetach(instance, out Ownership ownership))
                    continue;

                Root._disposal.Track(instance, ownership);
                return;
            }
        }

        internal IReadOnlyList<object> ResolveElements(Type elementType, in ResolutionRequest request)
        {
            ServiceIdentifier identifier = new ServiceIdentifier(elementType, request.Id);
            List<object> instances = new List<object>();

            ResolutionRequest elementRequest = new ResolutionRequest(
                identifier,
                request.ConsumerType,
                request.ConsumerInstance,
                request.MemberName,
                request.SiteKind);

            for (DIContainer current = this; current != null; current = current.Parent)
            {
                if (!current._index.TryGetValue(identifier, out RegistrationGroup group))
                    continue;

                IReadOnlyList<Registration> registrations = group.Registrations;

                for (int i = 0; i < registrations.Count; i++)
                {
                    Registration registration = registrations[i];

                    if (registration.Condition != null && !registration.Condition.Matches(in elementRequest))
                        continue;

                    CallSite site = new CallSite(registration, group.AllConditionsStatic, current);
                    instances.Add(current.GetOrActivate(site, in elementRequest, NO_ARGUMENTS, 0));
                }
            }

            return instances;
        }

        internal object ResolveRequest(in ResolutionRequest request, ResolutionPath path, int depth)
        {
            AssertUsable("resolve");
            MainThreadGuard.Assert();
            AssertDepth(in request, depth);

            CallSite site = _callSites.GetCallSite(in request, _chain, true);
            return GetOrActivate(site, in request, NO_ARGUMENTS, depth);
        }

        internal bool TryResolveRequest(
            in ResolutionRequest request,
            ResolutionPath path,
            int depth,
            out object instance)
        {
            AssertUsable("resolve");
            MainThreadGuard.Assert();
            AssertDepth(in request, depth);

            CallSite site = _callSites.GetCallSite(in request, _chain, false);

            if (site == null)
            {
                instance = null;
                return false;
            }

            instance = GetOrActivate(site, in request, NO_ARGUMENTS, depth);
            return true;
        }

        internal void RunInitialize()
        {
            _lifecycle.Initialize();
        }

        internal void ResolveNonLazy()
        {
            for (int i = 0; i < _registrations.Length; i++)
            {
                Registration registration = _registrations[i];

                if (!registration.NonLazy)
                    continue;

                ResolutionRequest request = ResolutionRequest.ForRoot(registration.ContractTypes[0], registration.Id);
                CallSite site = new CallSite(registration, true, this);
                GetOrActivate(site, in request, NO_ARGUMENTS, 0);
            }
        }

        private object GetOrActivate(
            CallSite site,
            in ResolutionRequest request,
            IReadOnlyList<object> arguments,
            int depth)
        {
            if (site.CacheLocation == CallSiteCacheLocation.None)
                return Activate(site, in request, arguments, depth);

            DIContainer owner = site.CacheLocation == CallSiteCacheLocation.Root ? site.Owner ?? Root : this;

            if (owner._store.TryGet(site.StoreSlot, out object cached) && _liveness.IsAlive(cached))
                return cached;

            return owner.CreateAndStore(site, in request, arguments, depth);
        }

        private object Activate(
            CallSite site,
            in ResolutionRequest request,
            IReadOnlyList<object> arguments,
            int depth)
        {
            GuardCycle(in request);
            _chain.Push(in request, site.Lifetime);

            try
            {
                ResolutionContext context = new ResolutionContext(this, in request, arguments, depth, _chain.Path);
                object created = site.Activator.Create(in context);

                _disposal.Track(created, site.Activator.Ownership);
                _lifecycle.Register(created);

                if (_pending.TryBegin(created))
                    site.Activator.Inject(created, in context);

                _observer.ResolutionCompleted(in request, site.Registration, created);
                return created;
            }
            finally
            {
                _chain.Pop();
            }
        }

        private object CreateAndStore(
            CallSite site,
            in ResolutionRequest request,
            IReadOnlyList<object> arguments,
            int depth)
        {
            GuardCycle(in request);
            _chain.Push(in request, site.Lifetime);

            try
            {
                ResolutionContext context = new ResolutionContext(this, in request, arguments, depth, _chain.Path);
                object created = site.Activator.Create(in context);

                _store.Set(site.StoreSlot, created);
                _disposal.Track(created, site.Activator.Ownership);
                _lifecycle.Register(created);

                try
                {
                    if (_pending.TryBegin(created))
                        site.Activator.Inject(created, in context);
                }
                catch
                {
                    _store.Evict(site.StoreSlot);
                    _disposal.Forget(created);
                    _pending.Forget(created);
                    throw;
                }

                _observer.ResolutionCompleted(in request, site.Registration, created);
                return created;
            }
            finally
            {
                _chain.Pop();
            }
        }

        private void GuardCycle(in ResolutionRequest request)
        {
            if (!_chain.Contains(request.Identifier))
                return;

            ResolutionPath path = _chain.Path.Snapshot();
            path.Push(request.ToFrame());
            throw new CircularDependencyException(path);
        }

        private void AssertDepth(in ResolutionRequest request, int depth)
        {
            if (depth <= _options.MaxResolutionDepth)
                return;

            throw new ResolutionDepthExceededException(
                _options.MaxResolutionDepth,
                request.ContractType,
                _chain.Path.Snapshot());
        }

        private void AssertUsable(string operation)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(DIContainer),
                    ResolutionMessage.ContainerPhase(BuildPhase.Disposed, BuildPhase.Built, operation));
        }

        #endregion
    }
}

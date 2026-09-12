using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal sealed class CallSiteFactory
    {
        #region Statements

        private static readonly CallSite[] NO_DEPENDENCIES = new CallSite[0];

        private readonly DIContainer _container;

        private readonly Dictionary<ResolutionCacheKey, CallSite> _cache =
            new Dictionary<ResolutionCacheKey, CallSite>();

        public CallSiteFactory(DIContainer container)
        {
            _container = container;
        }

        #endregion

        #region Methods

        public CallSite GetCallSite(in ResolutionRequest request, CallSiteChain chain, bool throwOnMissing)
        {
            return GetCallSite(in request, chain, throwOnMissing, false);
        }

        public CallSite GetCallSite(
            in ResolutionRequest request,
            CallSiteChain chain,
            bool throwOnMissing,
            bool analysis)
        {
            ResolutionCacheKey key = new ResolutionCacheKey(
                request.ContractType,
                request.Id,
                request.ConsumerType);

            if (_cache.TryGetValue(key, out CallSite cached))
                return cached;

            DIContainer owner = null;
            RegistrationGroup group = null;

            for (DIContainer current = _container; current != null; current = current.Parent)
            {
                if (!current.TryGetGroup(request.Identifier, out group))
                    continue;

                owner = current;
                break;
            }

            if (owner == null)
                return MissingOrCollection(in request, chain, key, throwOnMissing);

            List<SelectionFailure> rejected = new List<SelectionFailure>();
            Registration winner = RegistrationSelector.Select(
                group,
                in request,
                rejected,
                out List<Registration> ambiguous);

            if (ambiguous != null)
            {
                if (analysis && !group.AllConditionsStatic)
                    return null;

                throw new AmbiguousBindingException(
                    request.ContractType,
                    request.Id,
                    Snapshot(chain),
                    ambiguous,
                    ContainerDiagnostics.Describe(_container),
                    ContainerDiagnostics.DescribeInstallers(_container));
            }

            if (winner == null)
            {
                if (analysis && !group.AllConditionsStatic)
                    return null;

                if (throwOnMissing)
                    throw new BindingNotFoundException(
                        request.ContractType,
                        request.Id,
                        Snapshot(chain),
                        rejected,
                        ContainerDiagnostics.Describe(_container),
                        ContainerDiagnostics.DescribeInstallers(_container));

                return null;
            }

            CallSite site = new CallSite(winner, group.AllConditionsStatic, owner);

            if (chain.Contains(request.Identifier))
            {
                AssertBreakable(in request, chain, winner.Lifetime);
                return site;
            }

            chain.Push(in request, winner.Lifetime);

            try
            {
                BuildDependencies(site, chain);
            }
            finally
            {
                chain.Pop();
            }

            if (site.IsCacheable)
                _cache[key] = site;

            return site;
        }

        public void BuildDependencies(CallSite site, CallSiteChain chain)
        {
            IActivator activator = site.Activator;
            Type concreteType = activator.ProducedType;

            if (concreteType == null || concreteType.IsInterface || concreteType.IsAbstract)
            {
                site.Dependencies = NO_DEPENDENCIES;
                return;
            }

            List<CallSite> dependencies = new List<CallSite>();
            IReadOnlyList<InjectionParameter> declared = activator.DeclaredDependencies;

            for (int i = 0; i < declared.Count; i++)
                AddDependency(dependencies, declared[i], concreteType, InjectionSiteKind.Constructor, chain);

            InjectionPlan plan = _container.Injector.GetPlan(concreteType);

            for (int i = 0; i < plan.Fields.Length; i++)
                AddDependency(dependencies, plan.Fields[i].Parameter, concreteType, InjectionSiteKind.Field, chain);

            for (int i = 0; i < plan.Properties.Length; i++)
            {
                AddDependency(
                    dependencies,
                    plan.Properties[i].Parameter,
                    concreteType,
                    InjectionSiteKind.Property,
                    chain);
            }

            for (int i = 0; i < plan.Methods.Length; i++)
            {
                InjectionParameter[] parameters = plan.Methods[i].Parameters;

                for (int j = 0; j < parameters.Length; j++)
                    AddDependency(dependencies, parameters[j], concreteType, InjectionSiteKind.Method, chain);
            }

            for (int i = 0; i < plan.InjectableInterfaces.Length; i++)
            {
                InjectionParameter[] parameters = plan.InjectableInterfaces[i].Parameters;

                for (int j = 0; j < parameters.Length; j++)
                {
                    AddDependency(
                        dependencies,
                        parameters[j],
                        concreteType,
                        InjectionSiteKind.InjectableInterface,
                        chain);
                }
            }

            site.Dependencies = dependencies.ToArray();
        }

        public void Clear()
        {
            _cache.Clear();
        }


        private void AddDependency(
            List<CallSite> dependencies,
            in InjectionParameter parameter,
            Type consumerType,
            InjectionSiteKind siteKind,
            CallSiteChain chain)
        {
            ResolutionRequest request = new ResolutionRequest(
                parameter.Identifier,
                consumerType,
                null,
                parameter.Name,
                siteKind);

            CallSite dependency = GetCallSite(in request, chain, !parameter.Optional, true);

            if (dependency != null)
                dependencies.Add(dependency);
        }

        private CallSite MissingOrCollection(
            in ResolutionRequest request,
            CallSiteChain chain,
            in ResolutionCacheKey key,
            bool throwOnMissing)
        {
            if (CollectionContract.TryGetElementType(request.ContractType, out Type elementType))
            {
                CallSite site = BuildCollectionSite(request.ContractType, elementType);
                _cache[key] = site;

                return site;
            }

            if (throwOnMissing)
                throw new BindingNotFoundException(
                    request.ContractType,
                    request.Id,
                    Snapshot(chain),
                    null,
                    ContainerDiagnostics.Describe(_container),
                    ContainerDiagnostics.DescribeInstallers(_container));

            return null;
        }

        private CallSite BuildCollectionSite(Type contractType, Type elementType)
        {
            Registration registration = new Registration(
                new[] { contractType },
                contractType,
                Lifetime.Transient,
                null,
                new EnumerableActivator(contractType, elementType),
                null,
                false,
                BindingOrigin.Unknown);

            CallSite site = new CallSite(registration, true, _container);
            site.Dependencies = NO_DEPENDENCIES;

            return site;
        }

        private static void AssertBreakable(in ResolutionRequest request, CallSiteChain chain, Lifetime lifetime)
        {
            if (chain.IsBreakableCycle(request.Identifier, request.SiteKind, lifetime))
                return;

            ResolutionPath path = chain.Path.Snapshot();
            path.Push(request.ToFrame());

            throw new CircularDependencyException(path);
        }

        private static ResolutionPath Snapshot(CallSiteChain chain)
        {
            return chain == null ? new ResolutionPath() : chain.Path.Snapshot();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class ComponentActivator : IActivator
    {
        #region Statements

        private static readonly InjectionParameter[] NO_DEPENDENCIES = new InjectionParameter[0];

        private readonly IGameObjectFactory _factory;
        private readonly GameObjectPlacement _placement;
        private readonly IInjector _injector;
        private readonly bool _fromPrefab;
        private readonly Dictionary<Component, PendingHost> _pending = new Dictionary<Component, PendingHost>();

        public Type ProducedType { get; }

        public Ownership Ownership
        {
            get { return _factory.OwnsTheGameObject ? Ownership.UnityGameObject : Ownership.UnityComponent; }
        }

        public IReadOnlyList<InjectionParameter> DeclaredDependencies
        {
            get { return NO_DEPENDENCIES; }
        }

        public ComponentActivator(
            Type producedType,
            IGameObjectFactory factory,
            GameObjectPlacement placement,
            IInjector injector,
            bool fromPrefab)
        {
            ProducedType = producedType ?? throw new ArgumentNullException(nameof(producedType));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _placement = placement ?? throw new ArgumentNullException(nameof(placement));
            _injector = injector ?? throw new ArgumentNullException(nameof(injector));
            _fromPrefab = fromPrefab;
        }

        #endregion

        #region Methods

        public object Create(in ResolutionContext context)
        {
            GameObject host = _factory.Provide(_placement.Name ?? ProducedType.Name);
            bool wasActive = host.activeSelf;

            host.SetActive(false);

            if (!_fromPrefab)
            {
                Component added = host.AddComponent(ProducedType);
                _pending[added] = new PendingHost(host, wasActive);

                return added;
            }

            InjectSubtree(host, in context);

            Component found = host.GetComponentInChildren(ProducedType, true);

            if (found == null)
            {
                UnityEngine.Object.DestroyImmediate(host);

                throw new ActivationException(ProducedType, context.Path);
            }

            Place(host, in context, found);
            host.SetActive(wasActive);

            return found;
        }

        public void Inject(object instance, in ResolutionContext context)
        {
            if (!(instance is Component component))
                return;

            if (!_pending.TryGetValue(component, out PendingHost pending))
                return;

            _pending.Remove(component);

            try
            {
                _injector.Inject(component, in context);
                Place(pending.Host, in context, component);
            }
            finally
            {
                if (pending.Host != null)
                    pending.Host.SetActive(pending.WasActive);
            }
        }


        private void Place(GameObject host, in ResolutionContext context, Component component)
        {
            GameObjectPlacementApplier.Apply(host, _placement, ProducedType.Name);

            if (!_placement.DontDestroyOnLoad)
                return;

            GameObjectPlacementApplier.ApplyDontDestroyOnLoad(host);
            context.Container.AdoptToRoot(component);
        }

        private void InjectSubtree(GameObject host, in ResolutionContext context)
        {
            MonoBehaviour[] behaviours = host.GetComponentsInChildren<MonoBehaviour>(true);

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is null)
                    continue;

                context.Instantiator.Inject(behaviours[i]);
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class HierarchyComponentActivator : IActivator
    {
        #region Statements

        private static readonly InjectionParameter[] NO_DEPENDENCIES = new InjectionParameter[0];

        private readonly bool _includeInactive;
        private readonly IInjector _injector;

        public Type ProducedType { get; }

        public Ownership Ownership
        {
            get { return Ownership.None; }
        }

        public IReadOnlyList<InjectionParameter> DeclaredDependencies
        {
            get { return NO_DEPENDENCIES; }
        }

        public HierarchyComponentActivator(Type producedType, bool includeInactive, IInjector injector)
        {
            ProducedType = producedType ?? throw new ArgumentNullException(nameof(producedType));
            _includeInactive = includeInactive;
            _injector = injector ?? throw new ArgumentNullException(nameof(injector));
        }

        #endregion

        #region Methods

        public object Create(in ResolutionContext context)
        {
            if (!SceneScopeRegistry.TryGetScene(context.Container, out UnityEngine.SceneManagement.Scene scene))
                throw new BindingNotFoundException(ProducedType, null, context.Path, null);

            object found = UnityObjectFinder.FindInScene(scene, ProducedType, _includeInactive);

            if (found == null)
                throw new BindingNotFoundException(ProducedType, null, context.Path, null);

            return found;
        }

        public void Inject(object instance, in ResolutionContext context)
        {
            _injector.Inject(instance, in context);
        }

        #endregion
    }
}

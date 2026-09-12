using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class InstanceActivator : IActivator
    {
        #region Statements

        private static readonly InjectionParameter[] NO_DEPENDENCIES = new InjectionParameter[0];

        private readonly object _instance;
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

        public InstanceActivator(object instance, Type producedType, IInjector injector)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
            _injector = injector ?? throw new ArgumentNullException(nameof(injector));
            ProducedType = producedType ?? instance.GetType();
        }

        #endregion

        #region Methods

        public object Create(in ResolutionContext context)
        {
            return _instance;
        }

        public void Inject(object instance, in ResolutionContext context)
        {
            _injector.Inject(instance, in context);
        }

        #endregion
    }
}

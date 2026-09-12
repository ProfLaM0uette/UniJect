using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class ConstructorActivator : IActivator
    {
        #region Statements

        private readonly IInjector _injector;

        public Type ProducedType { get; }

        public Ownership Ownership
        {
            get { return Ownership.Managed; }
        }

        public IReadOnlyList<InjectionParameter> DeclaredDependencies { get; }

        public ConstructorActivator(Type producedType, IInjector injector)
        {
            ProducedType = producedType ?? throw new ArgumentNullException(nameof(producedType));
            _injector = injector ?? throw new ArgumentNullException(nameof(injector));
            DeclaredDependencies = _injector.GetPlan(producedType).ConstructorParameters;
        }

        #endregion

        #region Methods

        public object Create(in ResolutionContext context)
        {
            return _injector.CreateInstance(ProducedType, in context);
        }

        public void Inject(object instance, in ResolutionContext context)
        {
            _injector.Inject(instance, in context);
        }

        #endregion
    }
}

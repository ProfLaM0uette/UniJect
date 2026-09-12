using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class DelegateActivator : IActivator
    {
        #region Statements

        private static readonly InjectionParameter[] NO_DEPENDENCIES = new InjectionParameter[0];

        private readonly Func<IResolver, object> _factory;
        private readonly IInjector _injector;

        public Type ProducedType { get; }

        public Ownership Ownership
        {
            get { return Ownership.Managed; }
        }

        public IReadOnlyList<InjectionParameter> DeclaredDependencies
        {
            get { return NO_DEPENDENCIES; }
        }

        public DelegateActivator(Func<IResolver, object> factory, Type producedType, IInjector injector)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _injector = injector ?? throw new ArgumentNullException(nameof(injector));
            ProducedType = producedType ?? throw new ArgumentNullException(nameof(producedType));
        }

        #endregion

        #region Methods

        public object Create(in ResolutionContext context)
        {
            object created = _factory(context.Resolver);

            if (created == null)
                throw new ActivationException(ProducedType, context.Path);

            return created;
        }

        public void Inject(object instance, in ResolutionContext context)
        {
            _injector.Inject(instance, in context);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal sealed class PlaceholderFactoryActivator : IActivator
    {
        #region Statements

        private static readonly InjectionParameter[] NO_DEPENDENCIES = new InjectionParameter[0];

        private readonly Type _factoryType;
        private readonly ProductActivatorResolver _product;
        private readonly IInjector _injector;

        public Type ProducedType
        {
            get { return _factoryType; }
        }

        public Ownership Ownership
        {
            get { return Ownership.Managed; }
        }

        public IReadOnlyList<InjectionParameter> DeclaredDependencies
        {
            get { return NO_DEPENDENCIES; }
        }

        public PlaceholderFactoryActivator(Type factoryType, ProductActivatorResolver product, IInjector injector)
        {
            _factoryType = factoryType ?? throw new ArgumentNullException(nameof(factoryType));
            _product = product ?? throw new ArgumentNullException(nameof(product));
            _injector = injector ?? throw new ArgumentNullException(nameof(injector));
        }

        #endregion

        #region Methods

        public object Create(in ResolutionContext context)
        {
            object factory = _injector.CreateInstance(_factoryType, in context);

            if (!(factory is IPlaceholderFactoryBinding binding))
                throw new ActivationException(_factoryType, context.Path);

            IActivator productActivator = _product.Resolve(context.Container.Options);
            binding.Bind(new ActivatorProductFactory(context.Container, productActivator), context.Resolver);

            return factory;
        }

        public void Inject(object instance, in ResolutionContext context)
        {
            _injector.Inject(instance, in context);
        }

        #endregion
    }
}

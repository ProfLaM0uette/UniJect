using System;

namespace LaM0uette.UniJect
{
    internal sealed class ProductActivatorResolver
    {
        #region Statements

        private readonly Type _productType;
        private readonly IActivatorSource _declaredSource;
        private readonly IInjector _injector;

        private IActivator _activator;

        public Type ProductType
        {
            get { return _productType; }
        }

        public ProductActivatorResolver(Type productType, IActivatorSource declaredSource, IInjector injector)
        {
            _productType = productType ?? throw new ArgumentNullException(nameof(productType));
            _declaredSource = declaredSource;
            _injector = injector ?? throw new ArgumentNullException(nameof(injector));
        }

        #endregion

        #region Methods

        public IActivator Resolve(ContainerOptions options)
        {
            if (_activator != null)
                return _activator;

            IActivatorSource source = _declaredSource;

            if (source == null || source is ConstructorActivatorSource)
            {
                IActivatorSource fromRule = options.DefaultSourceRule?.Resolve(_productType);

                if (fromRule != null)
                    source = fromRule;
            }

            _activator = (source ?? ConstructorActivatorSource.Instance).Build(_productType, _injector);
            return _activator;
        }

        #endregion
    }
}

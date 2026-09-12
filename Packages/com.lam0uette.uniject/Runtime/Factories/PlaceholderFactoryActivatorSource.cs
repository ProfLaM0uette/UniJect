using System;

namespace LaM0uette.UniJect
{
    internal sealed class PlaceholderFactoryActivatorSource : IActivatorSource, IProductSourceHolder
    {
        #region Statements

        private readonly Type _productType;
        private readonly Type[] _parameterTypes;

        public Type ProductType
        {
            get { return _productType; }
        }

        public IActivatorSource ProductSource { get; set; }

        public PlaceholderFactoryActivatorSource(Type productType, Type[] parameterTypes)
        {
            _productType = productType;
            _parameterTypes = parameterTypes;
        }

        #endregion

        #region Methods

        public IActivator Build(Type concreteType, IInjector injector)
        {
            FactoryParameterCheck.Assert(_productType, _parameterTypes, ProductSource);

            ProductActivatorResolver product = new ProductActivatorResolver(_productType, ProductSource, injector);
            return new PlaceholderFactoryActivator(concreteType, product, injector);
        }

        #endregion
    }
}

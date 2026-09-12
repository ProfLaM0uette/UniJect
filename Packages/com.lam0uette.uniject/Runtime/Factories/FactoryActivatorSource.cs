using System;

namespace LaM0uette.UniJect
{
    internal sealed class FactoryActivatorSource : IActivatorSource, IProductSourceHolder
    {
        #region Statements

        private readonly Type _productType;
        private readonly Func<IProductFactory, object> _adapter;
        private readonly Type[] _parameterTypes;

        public Type ProductType
        {
            get { return _productType; }
        }

        public IActivatorSource ProductSource { get; set; }

        public FactoryActivatorSource(
            Type productType,
            Func<IProductFactory, object> adapter,
            Type[] parameterTypes)
        {
            _productType = productType;
            _adapter = adapter;
            _parameterTypes = parameterTypes;
        }

        #endregion

        #region Methods

        public IActivator Build(Type concreteType, IInjector injector)
        {
            FactoryParameterCheck.Assert(_productType, _parameterTypes, ProductSource);

            ProductActivatorResolver product = new ProductActivatorResolver(_productType, ProductSource, injector);
            return new FactoryActivator(concreteType, product, _adapter);
        }

        #endregion
    }
}

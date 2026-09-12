using System;

namespace LaM0uette.UniJect
{
    public abstract class PlaceholderFactoryBase<TProduct> : IPlaceholderFactoryBinding
    {
        #region Statements

        protected IProductFactory ProductFactory { get; private set; }
        protected IResolver Resolver { get; private set; }

        #endregion

        #region IPlaceholderFactoryBinding

        void IPlaceholderFactoryBinding.Bind(IProductFactory productFactory, IResolver resolver)
        {
            ProductFactory = productFactory ?? throw new ArgumentNullException(nameof(productFactory));
            Resolver = resolver;
        }

        #endregion
    }
}

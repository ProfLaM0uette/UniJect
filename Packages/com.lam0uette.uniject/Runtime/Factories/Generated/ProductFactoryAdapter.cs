namespace LaM0uette.UniJect
{
    [System.CodeDom.Compiler.GeneratedCode("UniJect.ArityGenerator", "1.0")]
    internal sealed class ProductFactoryAdapter<TProduct> : IFactory<TProduct>
    {
        private readonly IProductFactory _productFactory;

        public ProductFactoryAdapter(IProductFactory productFactory)
        {
            _productFactory = productFactory;
        }

        public TProduct Create()
        {
            return (TProduct)_productFactory.Create(ArgumentBag.EMPTY);
        }
    }
}

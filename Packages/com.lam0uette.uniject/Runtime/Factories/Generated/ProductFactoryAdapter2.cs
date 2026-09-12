namespace LaM0uette.UniJect
{
    [System.CodeDom.Compiler.GeneratedCode("UniJect.ArityGenerator", "1.0")]
    internal sealed class ProductFactoryAdapter<TProduct, P1, P2> : IFactory<TProduct, P1, P2>
    {
        private readonly IProductFactory _productFactory;

        public ProductFactoryAdapter(IProductFactory productFactory)
        {
            _productFactory = productFactory;
        }

        public TProduct Create(P1 p1, P2 p2)
        {
            return (TProduct)_productFactory.Create(new object[] { p1, p2 });
        }
    }
}

namespace LaM0uette.UniJect
{
    [System.CodeDom.Compiler.GeneratedCode("UniJect.ArityGenerator", "1.0")]
    internal sealed class ProductFactoryAdapter<TProduct, P1, P2, P3, P4, P5> : IFactory<TProduct, P1, P2, P3, P4, P5>
    {
        private readonly IProductFactory _productFactory;

        public ProductFactoryAdapter(IProductFactory productFactory)
        {
            _productFactory = productFactory;
        }

        public TProduct Create(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
        {
            return (TProduct)_productFactory.Create(new object[] { p1, p2, p3, p4, p5 });
        }
    }
}

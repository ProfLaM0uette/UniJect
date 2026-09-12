namespace LaM0uette.UniJect
{
    [System.CodeDom.Compiler.GeneratedCode("UniJect.ArityGenerator", "1.0")]
    public abstract class PlaceholderFactory<TProduct, P1, P2, P3> : PlaceholderFactoryBase<TProduct>, IFactory<TProduct, P1, P2, P3>
    {
        public virtual TProduct Create(P1 p1, P2 p2, P3 p3)
        {
            return (TProduct)ProductFactory.Create(new object[] { p1, p2, p3 });
        }
    }
}

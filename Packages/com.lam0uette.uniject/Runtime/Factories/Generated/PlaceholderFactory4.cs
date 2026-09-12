namespace LaM0uette.UniJect
{
    [System.CodeDom.Compiler.GeneratedCode("UniJect.ArityGenerator", "1.0")]
    public abstract class PlaceholderFactory<TProduct, P1, P2, P3, P4> : PlaceholderFactoryBase<TProduct>, IFactory<TProduct, P1, P2, P3, P4>
    {
        public virtual TProduct Create(P1 p1, P2 p2, P3 p3, P4 p4)
        {
            return (TProduct)ProductFactory.Create(new object[] { p1, p2, p3, p4 });
        }
    }
}

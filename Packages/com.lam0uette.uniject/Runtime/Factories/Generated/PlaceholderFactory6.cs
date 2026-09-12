namespace LaM0uette.UniJect
{
    [System.CodeDom.Compiler.GeneratedCode("UniJect.ArityGenerator", "1.0")]
    public abstract class PlaceholderFactory<TProduct, P1, P2, P3, P4, P5, P6> : PlaceholderFactoryBase<TProduct>, IFactory<TProduct, P1, P2, P3, P4, P5, P6>
    {
        public virtual TProduct Create(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
        {
            return (TProduct)ProductFactory.Create(new object[] { p1, p2, p3, p4, p5, p6 });
        }
    }
}

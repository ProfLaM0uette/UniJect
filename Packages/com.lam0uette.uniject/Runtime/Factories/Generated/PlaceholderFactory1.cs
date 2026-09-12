namespace LaM0uette.UniJect
{
    [System.CodeDom.Compiler.GeneratedCode("UniJect.ArityGenerator", "1.0")]
    public abstract class PlaceholderFactory<TProduct, P1> : PlaceholderFactoryBase<TProduct>, IFactory<TProduct, P1>
    {
        public virtual TProduct Create(P1 p1)
        {
            return (TProduct)ProductFactory.Create(new object[] { p1 });
        }
    }
}

namespace LaM0uette.UniJect
{
    [System.CodeDom.Compiler.GeneratedCode("UniJect.ArityGenerator", "1.0")]
    public abstract class PlaceholderFactory<TProduct> : PlaceholderFactoryBase<TProduct>, IFactory<TProduct>
    {
        public virtual TProduct Create()
        {
            return (TProduct)ProductFactory.Create(ArgumentBag.EMPTY);
        }
    }
}

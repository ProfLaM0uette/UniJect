namespace LaM0uette.UniJect
{
    internal interface IPlaceholderFactoryBinding
    {
        void Bind(IProductFactory productFactory, IResolver resolver);
    }
}

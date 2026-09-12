namespace LaM0uette.UniJect
{
    public sealed class SingletonWithScopedDependency
    {
        public ScopedService Service { get; }

        public SingletonWithScopedDependency(ScopedService service)
        {
            Service = service;
        }
    }
}

namespace LaM0uette.UniJect
{
    public sealed class WindowConsumer
    {
        public IWindowService Service { get; }

        public WindowConsumer(IWindowService service)
        {
            Service = service;
        }
    }
}

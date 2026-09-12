namespace LaM0uette.UniJect
{
    public sealed class InjectableConsumer : IInjectable<ILog>
    {
        public ILog Log { get; private set; }

        public void Inject(ILog log)
        {
            Log = log;
        }
    }
}

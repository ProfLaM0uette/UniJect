namespace LaM0uette.UniJect
{
    public sealed class FieldConsumer
    {
        [Inject] private ILog _log;

        public ILog Log
        {
            get { return _log; }
        }
    }
}

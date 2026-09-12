namespace LaM0uette.UniJect
{
    public sealed class SecondInitializable : IInitializable
    {
        private readonly InitializationRecorder _recorder;

        public SecondInitializable(InitializationRecorder recorder, FirstInitializable first)
        {
            _recorder = recorder;
        }

        public void Initialize()
        {
            _recorder.Order.Add(nameof(SecondInitializable));
        }
    }
}

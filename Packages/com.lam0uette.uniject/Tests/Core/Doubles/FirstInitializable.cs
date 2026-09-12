namespace LaM0uette.UniJect
{
    public sealed class FirstInitializable : IInitializable
    {
        private readonly InitializationRecorder _recorder;

        public FirstInitializable(InitializationRecorder recorder)
        {
            _recorder = recorder;
        }

        public void Initialize()
        {
            _recorder.Order.Add(nameof(FirstInitializable));
        }
    }
}

namespace LaM0uette.UniJect
{
    public sealed class LateRegisteredTickable : ITickable
    {
        private readonly TickRecorder _recorder;

        public LateRegisteredTickable(TickRecorder recorder)
        {
            _recorder = recorder;
        }

        public void Tick(float deltaTime)
        {
            _recorder.Order.Add(nameof(LateRegisteredTickable));
        }
    }
}

namespace LaM0uette.UniJect
{
    public sealed class CountingTickable : ITickable, IFixedTickable, ILateTickable
    {
        private readonly TickRecorder _recorder;

        public CountingTickable(TickRecorder recorder)
        {
            _recorder = recorder;
        }

        public void Tick(float deltaTime)
        {
            _recorder.TickCount++;
            _recorder.Order.Add(nameof(CountingTickable));
        }

        public void FixedTick(float fixedDeltaTime)
        {
            _recorder.FixedTickCount++;
        }

        public void LateTick(float deltaTime)
        {
            _recorder.LateTickCount++;
        }
    }
}

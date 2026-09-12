namespace LaM0uette.UniJect
{
    public sealed class FrameTickable : ITickable, IFixedTickable, ILateTickable
    {
        private readonly FrameTickRecorder _recorder;

        public FrameTickable(FrameTickRecorder recorder)
        {
            _recorder = recorder;
        }

        public void Tick(float deltaTime)
        {
            _recorder.TickCount++;
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

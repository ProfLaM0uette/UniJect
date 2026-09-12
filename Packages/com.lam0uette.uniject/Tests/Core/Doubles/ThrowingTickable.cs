using System;

namespace LaM0uette.UniJect
{
    public sealed class ThrowingTickable : ITickable
    {
        private readonly TickRecorder _recorder;

        public ThrowingTickable(TickRecorder recorder)
        {
            _recorder = recorder;
        }

        public void Tick(float deltaTime)
        {
            _recorder.Order.Add(nameof(ThrowingTickable));
            throw new InvalidOperationException("UniJect test: this tickable always throws.");
        }
    }
}

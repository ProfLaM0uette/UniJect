using System;

namespace LaM0uette.UniJect
{
    public sealed class FirstDisposable : IDisposable
    {
        private readonly DisposalRecorder _recorder;

        public FirstDisposable(DisposalRecorder recorder)
        {
            _recorder = recorder;
        }

        public void Dispose()
        {
            _recorder.Order.Add(nameof(FirstDisposable));
        }
    }
}

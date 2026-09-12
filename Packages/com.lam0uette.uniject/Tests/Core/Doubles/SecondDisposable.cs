using System;

namespace LaM0uette.UniJect
{
    public sealed class SecondDisposable : IDisposable
    {
        private readonly DisposalRecorder _recorder;

        public SecondDisposable(DisposalRecorder recorder, FirstDisposable first)
        {
            _recorder = recorder;
        }

        public void Dispose()
        {
            _recorder.Order.Add(nameof(SecondDisposable));
        }
    }
}

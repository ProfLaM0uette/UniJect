using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class DisposalRecorder
    {
        public List<string> Order { get; } = new List<string>();
    }
}

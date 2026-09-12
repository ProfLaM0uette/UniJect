using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class InitializationRecorder
    {
        public List<string> Order { get; } = new List<string>();
    }
}

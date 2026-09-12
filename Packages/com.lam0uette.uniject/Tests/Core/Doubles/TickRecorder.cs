using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class TickRecorder
    {
        public List<string> Order { get; } = new List<string>();
        public int TickCount { get; set; }
        public int FixedTickCount { get; set; }
        public int LateTickCount { get; set; }
    }
}

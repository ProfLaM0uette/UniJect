using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class MemoryLog : ILog
    {
        public List<string> Lines { get; } = new List<string>();

        public void Write(string message)
        {
            Lines.Add(message);
        }
    }
}

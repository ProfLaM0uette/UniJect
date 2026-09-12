using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class MultiService : ILog, ISaveable
    {
        public static int CreationCount;

        public List<string> Lines { get; } = new List<string>();

        public MultiService()
        {
            CreationCount++;
        }

        public void Write(string message)
        {
            Lines.Add(message);
        }

        public string Save()
        {
            return Lines.Count.ToString();
        }
    }
}

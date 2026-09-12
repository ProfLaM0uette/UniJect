using UnityEngine;

namespace LaM0uette.UniJect
{
    internal readonly struct PendingHost
    {
        public GameObject Host { get; }
        public bool WasActive { get; }

        public PendingHost(GameObject host, bool wasActive)
        {
            Host = host;
            WasActive = wasActive;
        }
    }
}

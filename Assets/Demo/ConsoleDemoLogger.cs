using UnityEngine;

namespace UniJect.Demo
{
    public sealed class ConsoleDemoLogger : IDemoLogger
    {
        #region Methods

        public void Line(string message)
        {
            Debug.Log("[UniJect demo] " + message);
        }

        #endregion
    }
}

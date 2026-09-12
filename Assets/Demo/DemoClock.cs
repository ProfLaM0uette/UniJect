using UnityEngine;

namespace UniJect.Demo
{
    public sealed class DemoClock : IClock
    {
        #region Statements

        public float Now
        {
            get { return Time.time; }
        }

        #endregion
    }
}

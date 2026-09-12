using UnityEngine;

namespace LaM0uette.UniJect.Samples.Installers
{
    public sealed class UnityClock : IClock
    {
        #region Statements

        public float Now
        {
            get { return Time.time; }
        }

        #endregion
    }
}

using UnityEngine;

namespace LaM0uette.UniJect.Samples.GameObjects
{
    public sealed class Turret : MonoBehaviour
    {
        #region Statements

        [Inject] private ITargeting _targeting;

        private void Awake()
        {
            Debug.Log(name + " woke up with targeting: " + (_targeting != null));
        }

        #endregion
    }
}

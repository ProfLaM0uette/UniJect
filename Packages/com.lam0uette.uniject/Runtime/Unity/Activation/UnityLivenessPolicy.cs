using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class UnityLivenessPolicy : IInstanceLivenessPolicy
    {
        #region Statements

        public static readonly UnityLivenessPolicy Instance = new UnityLivenessPolicy();

        private UnityLivenessPolicy()
        {
        }

        #endregion

        #region Methods

        public bool IsAlive(object instance)
        {
            if (instance is Object unityObject)
                return unityObject != null;

            return true;
        }

        #endregion
    }
}

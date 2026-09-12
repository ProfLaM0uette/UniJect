using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class UnityObjectReleaser : IInstanceReleaser
    {
        #region Statements

        public static readonly UnityObjectReleaser Instance = new UnityObjectReleaser();

        private UnityObjectReleaser()
        {
        }

        #endregion

        #region Methods

        public bool TryRelease(object instance, Ownership ownership)
        {
            if (!(instance is Component component))
                return false;

            if (component == null)
                return false;

            if (ownership == Ownership.UnityComponent)
            {
                Destroy(component);
                return true;
            }

            if (ownership != Ownership.UnityGameObject)
                return false;

            Destroy(component.gameObject);
            return true;
        }


        private static void Destroy(Object target)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(target);
                return;
            }

            Object.DestroyImmediate(target);
        }

        #endregion
    }
}

using UnityEngine;

namespace LaM0uette.UniJect
{
    internal static class InactiveInstantiationScope
    {
        #region Statements

        private const string HOLDER_NAME = "[UniJect.Instantiation]";

        private static GameObject _holder;

        public static Transform Holder
        {
            get { return Acquire(); }
        }

        #endregion

        #region Methods

        public static void Reset()
        {
            if (_holder != null)
                Object.DestroyImmediate(_holder);

            _holder = null;
        }


        private static Transform Acquire()
        {
            if (_holder != null)
                return _holder.transform;

            _holder = new GameObject(HOLDER_NAME) { hideFlags = HideFlags.HideAndDontSave };
            _holder.SetActive(false);

            return _holder.transform;
        }

        #endregion
    }
}

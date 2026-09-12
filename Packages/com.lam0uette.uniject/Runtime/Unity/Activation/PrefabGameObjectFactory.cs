using System;
using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class PrefabGameObjectFactory : IGameObjectFactory
    {
        #region Statements

        private readonly GameObject _prefab;

        public bool OwnsTheGameObject
        {
            get { return true; }
        }

        public PrefabGameObjectFactory(GameObject prefab)
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            _prefab = prefab;
        }

        #endregion

        #region Methods

        public GameObject Provide(string defaultName)
        {
            return UnityEngine.Object.Instantiate(_prefab, InactiveInstantiationScope.Holder, false);
        }

        #endregion
    }
}

using System;
using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class ResourcePrefabGameObjectFactory : IGameObjectFactory
    {
        #region Statements

        private readonly string _resourcePath;

        private GameObject _prefab;

        public bool OwnsTheGameObject
        {
            get { return true; }
        }

        public ResourcePrefabGameObjectFactory(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
                throw new ArgumentException("UniJect: the resource path is empty.", nameof(resourcePath));

            _resourcePath = resourcePath;
        }

        #endregion

        #region Methods

        public GameObject Provide(string defaultName)
        {
            if (_prefab == null)
                _prefab = Resources.Load<GameObject>(_resourcePath);

            if (_prefab == null)
                throw new ActivationException(typeof(GameObject), null);

            return UnityEngine.Object.Instantiate(_prefab, InactiveInstantiationScope.Holder, false);
        }

        #endregion
    }
}

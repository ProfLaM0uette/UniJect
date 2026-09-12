using System;
using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class ExistingGameObjectFactory : IGameObjectFactory
    {
        #region Statements

        private readonly GameObject _gameObject;

        public bool OwnsTheGameObject
        {
            get { return false; }
        }

        public ExistingGameObjectFactory(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            _gameObject = gameObject;
        }

        #endregion

        #region Methods

        public GameObject Provide(string defaultName)
        {
            return _gameObject;
        }

        #endregion
    }
}

using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class NewGameObjectFactory : IGameObjectFactory
    {
        #region Statements

        public bool OwnsTheGameObject
        {
            get { return true; }
        }

        #endregion

        #region Methods

        public GameObject Provide(string defaultName)
        {
            return new GameObject(defaultName);
        }

        #endregion
    }
}

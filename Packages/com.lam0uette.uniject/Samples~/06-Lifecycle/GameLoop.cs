using System;
using UnityEngine;

namespace LaM0uette.UniJect.Samples.Lifecycle
{
    public sealed class GameLoop : IInitializable, ITickable, IDisposable
    {
        #region Statements

        private float _elapsed;

        #endregion

        #region Methods

        public void Initialize()
        {
            Debug.Log("GameLoop initialised before the first frame.");
        }

        public void Tick(float deltaTime)
        {
            _elapsed += deltaTime;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Debug.Log("GameLoop ran for " + _elapsed + " seconds.");
        }

        #endregion
    }
}

using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class MonoThing : MonoBehaviour
    {
        #region Statements

        [Inject] private IGreeter _greeter;

        public IGreeter Greeter
        {
            get { return _greeter; }
        }

        public bool SawGreeterInAwake { get; private set; }
        public int AwakeCount { get; private set; }

        private void Awake()
        {
            SawGreeterInAwake = _greeter != null;
            AwakeCount++;
        }

        #endregion
    }
}

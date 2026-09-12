using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class GreetedBehaviour : MonoBehaviour
    {
        #region Statements

        [Inject] private IGreeter _greeter;

        public IGreeter Greeter
        {
            get { return _greeter; }
        }

        #endregion
    }
}

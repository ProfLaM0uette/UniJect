using UnityEngine;

namespace LaM0uette.UniJect.Samples.Hello
{
    public sealed class Greeter : MonoBehaviour
    {
        #region Statements

        [Inject] private IGreetingService _greetings;

        private void Start()
        {
            Debug.Log(_greetings.Greet(name));
        }

        #endregion
    }
}

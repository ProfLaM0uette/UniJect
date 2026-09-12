using LaM0uette.UniJect;
using UnityEngine;

namespace UniJect.Demo
{
    public sealed class Spinner : MonoBehaviour
    {
        #region Statements

        [Inject] private IDemoLogger _logger;

        public bool SawLoggerInAwake { get; private set; }

        private void Awake()
        {
            SawLoggerInAwake = _logger != null;
            _logger?.Line("Spinner.Awake ran, and its dependency was already there: " + SawLoggerInAwake);
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, 90f * Time.deltaTime);
        }

        #endregion
    }
}

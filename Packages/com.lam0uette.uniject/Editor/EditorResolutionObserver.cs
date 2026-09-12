using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class EditorResolutionObserver : IResolutionObserver
    {
        #region Statements

        public static readonly EditorResolutionObserver Instance = new EditorResolutionObserver();

        private readonly Dictionary<Registration, int> _resolutions = new Dictionary<Registration, int>();

        private EditorResolutionObserver()
        {
        }

        #endregion

        #region Methods

        public int CountFor(Registration registration)
        {
            return registration != null && _resolutions.TryGetValue(registration, out int count) ? count : 0;
        }

        public void Clear()
        {
            _resolutions.Clear();
        }

        public void ResolutionCompleted(in ResolutionRequest request, Registration registration, object instance)
        {
            if (registration == null)
                return;

            _resolutions.TryGetValue(registration, out int count);
            _resolutions[registration] = count + 1;
        }

        public void InjectionCompleted(object target, Registration registration)
        {
        }

        #endregion
    }
}

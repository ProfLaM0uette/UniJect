namespace LaM0uette.UniJect
{
    public sealed class NullResolutionObserver : IResolutionObserver
    {
        #region Statements

        public static readonly NullResolutionObserver Instance = new NullResolutionObserver();

        private NullResolutionObserver()
        {
        }

        #endregion

        #region Methods

        public void ResolutionCompleted(in ResolutionRequest request, Registration registration, object instance)
        {
        }

        public void InjectionCompleted(object target, Registration registration)
        {
        }

        #endregion
    }
}

using System;

namespace LaM0uette.UniJect
{
    public sealed class DisposableReleaser : IInstanceReleaser
    {
        #region Statements

        public static readonly DisposableReleaser Instance = new DisposableReleaser();

        private DisposableReleaser()
        {
        }

        #endregion

        #region Methods

        public bool TryRelease(object instance, Ownership ownership)
        {
            if (ownership == Ownership.None)
                return false;

            if (!(instance is IDisposable disposable))
                return false;

            disposable.Dispose();
            return true;
        }

        #endregion
    }
}

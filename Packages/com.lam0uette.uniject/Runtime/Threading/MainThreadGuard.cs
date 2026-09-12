using System;
using System.Diagnostics;
using System.Threading;

namespace LaM0uette.UniJect
{
    public static class MainThreadGuard
    {
        #region Statements

        private static int _mainThreadId = -1;

        #endregion

        #region Methods

        public static void Capture()
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public static void Release()
        {
            _mainThreadId = -1;
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEBUG")]
        [Conditional("UNIJECT_ASSERT_MAIN_THREAD")]
        public static void Assert()
        {
            if (_mainThreadId < 0 || Thread.CurrentThread.ManagedThreadId == _mainThreadId)
                return;

            throw new InvalidOperationException(
                "UniJect: the container was used from thread " + Thread.CurrentThread.ManagedThreadId +
                " but it is owned by thread " + _mainThreadId +
                ". Resolve on the main thread, or turn AssertMainThread off if you know what you are doing.");
        }

        #endregion
    }
}

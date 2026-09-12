using System.Runtime.CompilerServices;

namespace LaM0uette.UniJect
{
    internal sealed class PendingInjectionSet
    {
        #region Statements

        private static readonly object MARKER = new object();

        private readonly ConditionalWeakTable<object, object> _injected = new ConditionalWeakTable<object, object>();

        #endregion

        #region Methods

        public bool TryBegin(object target)
        {
            if (target == null)
                return false;

            if (_injected.TryGetValue(target, out object _))
                return false;

            _injected.Add(target, MARKER);
            return true;
        }

        public void Forget(object target)
        {
            if (target != null)
                _injected.Remove(target);
        }

        #endregion
    }
}

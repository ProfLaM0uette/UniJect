using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal sealed class DisposalTracker : IDisposalTracker
    {
        #region Statements

        private readonly List<object> _instances = new List<object>();
        private readonly List<Ownership> _ownerships = new List<Ownership>();
        private readonly IReadOnlyList<IInstanceReleaser> _releasers;

        public DisposalTracker(IReadOnlyList<IInstanceReleaser> releasers)
        {
            _releasers = releasers ?? throw new ArgumentNullException(nameof(releasers));
        }

        #endregion

        #region Methods

        public void Track(object instance, Ownership ownership)
        {
            if (instance == null || ownership == Ownership.None)
                return;

            _instances.Add(instance);
            _ownerships.Add(ownership);
        }

        public void Forget(object instance)
        {
            if (instance == null)
                return;

            for (int i = _instances.Count - 1; i >= 0; i--)
            {
                if (!ReferenceEquals(_instances[i], instance))
                    continue;

                _instances.RemoveAt(i);
                _ownerships.RemoveAt(i);
                return;
            }
        }

        public bool TryDetach(object instance, out Ownership ownership)
        {
            for (int i = _instances.Count - 1; i >= 0; i--)
            {
                if (!ReferenceEquals(_instances[i], instance))
                    continue;

                ownership = _ownerships[i];
                _instances.RemoveAt(i);
                _ownerships.RemoveAt(i);
                return true;
            }

            ownership = Ownership.None;
            return false;
        }

        public void DisposeAll()
        {
            List<Exception> failures = null;

            for (int i = _instances.Count - 1; i >= 0; i--)
            {
                object instance = _instances[i];
                Ownership ownership = _ownerships[i];

                for (int j = 0; j < _releasers.Count; j++)
                {
                    try
                    {
                        _releasers[j].TryRelease(instance, ownership);
                    }
                    catch (Exception exception)
                    {
                        failures = failures ?? new List<Exception>();
                        failures.Add(exception);
                    }
                }
            }

            _instances.Clear();
            _ownerships.Clear();

            if (failures != null)
                throw new AggregateException(failures);
        }

        #endregion
    }
}

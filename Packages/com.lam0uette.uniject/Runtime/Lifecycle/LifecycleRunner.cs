using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal sealed class LifecycleRunner
    {
        #region Statements

        private readonly List<IInitializable> _initializables = new List<IInitializable>();

        private int _initializedCount;

        #endregion

        #region Methods

        public void Register(object instance)
        {
            if (instance is IInitializable initializable)
                _initializables.Add(initializable);
        }

        public void Initialize()
        {
            List<Exception> failures = null;

            while (_initializedCount < _initializables.Count)
            {
                IInitializable initializable = _initializables[_initializedCount];
                _initializedCount++;

                try
                {
                    initializable.Initialize();
                }
                catch (Exception exception)
                {
                    failures = failures ?? new List<Exception>();
                    failures.Add(exception);
                }
            }

            if (failures != null)
                throw new AggregateException(failures);
        }

        public void Clear()
        {
            _initializables.Clear();
            _initializedCount = 0;
        }

        #endregion
    }
}

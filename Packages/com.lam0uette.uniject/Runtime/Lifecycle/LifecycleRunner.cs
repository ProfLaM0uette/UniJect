using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal sealed class LifecycleRunner
    {
        #region Statements

        private readonly List<IInitializable> _initializables = new List<IInitializable>();
        private readonly List<ITickable> _tickables = new List<ITickable>();
        private readonly List<IFixedTickable> _fixedTickables = new List<IFixedTickable>();
        private readonly List<ILateTickable> _lateTickables = new List<ILateTickable>();

        private readonly List<ITickable> _tickSnapshot = new List<ITickable>();
        private readonly List<IFixedTickable> _fixedSnapshot = new List<IFixedTickable>();
        private readonly List<ILateTickable> _lateSnapshot = new List<ILateTickable>();

        private int _initializedCount;

        public bool HasTickables
        {
            get { return _tickables.Count > 0 || _fixedTickables.Count > 0 || _lateTickables.Count > 0; }
        }

        #endregion

        #region Methods

        public void Register(object instance)
        {
            if (instance is IInitializable initializable)
                _initializables.Add(initializable);

            if (instance is ITickable tickable)
                _tickables.Add(tickable);

            if (instance is IFixedTickable fixedTickable)
                _fixedTickables.Add(fixedTickable);

            if (instance is ILateTickable lateTickable)
                _lateTickables.Add(lateTickable);
        }

        public void Unregister(object instance)
        {
            if (instance is ITickable tickable)
                _tickables.Remove(tickable);

            if (instance is IFixedTickable fixedTickable)
                _fixedTickables.Remove(fixedTickable);

            if (instance is ILateTickable lateTickable)
                _lateTickables.Remove(lateTickable);
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

        public void Tick(float deltaTime, Action<Exception> onError)
        {
            _tickSnapshot.Clear();
            _tickSnapshot.AddRange(_tickables);

            for (int i = 0; i < _tickSnapshot.Count; i++)
            {
                try
                {
                    _tickSnapshot[i].Tick(deltaTime);
                }
                catch (Exception exception)
                {
                    onError?.Invoke(exception);
                }
            }

            _tickSnapshot.Clear();
        }

        public void FixedTick(float fixedDeltaTime, Action<Exception> onError)
        {
            _fixedSnapshot.Clear();
            _fixedSnapshot.AddRange(_fixedTickables);

            for (int i = 0; i < _fixedSnapshot.Count; i++)
            {
                try
                {
                    _fixedSnapshot[i].FixedTick(fixedDeltaTime);
                }
                catch (Exception exception)
                {
                    onError?.Invoke(exception);
                }
            }

            _fixedSnapshot.Clear();
        }

        public void LateTick(float deltaTime, Action<Exception> onError)
        {
            _lateSnapshot.Clear();
            _lateSnapshot.AddRange(_lateTickables);

            for (int i = 0; i < _lateSnapshot.Count; i++)
            {
                try
                {
                    _lateSnapshot[i].LateTick(deltaTime);
                }
                catch (Exception exception)
                {
                    onError?.Invoke(exception);
                }
            }

            _lateSnapshot.Clear();
        }

        public void Clear()
        {
            _initializables.Clear();
            _tickables.Clear();
            _fixedTickables.Clear();
            _lateTickables.Clear();
            _tickSnapshot.Clear();
            _fixedSnapshot.Clear();
            _lateSnapshot.Clear();
            _initializedCount = 0;
        }

        #endregion
    }
}

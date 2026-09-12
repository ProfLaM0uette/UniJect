using System;

namespace LaM0uette.UniJect
{
    internal sealed class InstanceStore : IInstanceStore
    {
        #region Statements

        private static readonly object EMPTY = new object();

        private readonly object[] _slots;

        public InstanceStore(int size)
        {
            _slots = new object[size < 0 ? 0 : size];

            for (int i = 0; i < _slots.Length; i++)
                _slots[i] = EMPTY;
        }

        #endregion

        #region Methods

        public bool TryGet(int slot, out object instance)
        {
            if (slot < 0 || slot >= _slots.Length)
            {
                instance = null;
                return false;
            }

            object stored = _slots[slot];

            if (ReferenceEquals(stored, EMPTY))
            {
                instance = null;
                return false;
            }

            instance = stored;
            return true;
        }

        public void Set(int slot, object instance)
        {
            if (slot < 0 || slot >= _slots.Length)
                throw new ArgumentOutOfRangeException(nameof(slot));

            _slots[slot] = instance;
        }

        public void Evict(int slot)
        {
            if (slot >= 0 && slot < _slots.Length)
                _slots[slot] = EMPTY;
        }

        public void Clear()
        {
            for (int i = 0; i < _slots.Length; i++)
                _slots[i] = EMPTY;
        }

        #endregion
    }
}

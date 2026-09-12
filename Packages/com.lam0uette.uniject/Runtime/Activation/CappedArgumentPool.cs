using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal sealed class CappedArgumentPool : IArgumentPool
    {
        #region Statements

        private const int MAX_SIZE = 8;
        private const int MAX_PER_BUCKET = 4;

        public static readonly CappedArgumentPool Instance = new CappedArgumentPool();

        private readonly List<object[]>[] _buckets = new List<object[]>[MAX_SIZE + 1];

        private CappedArgumentPool()
        {
            for (int i = 0; i <= MAX_SIZE; i++)
                _buckets[i] = new List<object[]>();
        }

        #endregion

        #region Methods

        public object[] Rent(int size)
        {
            if (size < 0 || size > MAX_SIZE)
                return new object[size < 0 ? 0 : size];

            List<object[]> bucket = _buckets[size];

            if (bucket.Count == 0)
                return new object[size];

            object[] rented = bucket[bucket.Count - 1];
            bucket.RemoveAt(bucket.Count - 1);

            return rented;
        }

        public void Return(object[] arguments)
        {
            if (arguments == null || arguments.Length > MAX_SIZE)
                return;

            for (int i = 0; i < arguments.Length; i++)
                arguments[i] = null;

            List<object[]> bucket = _buckets[arguments.Length];

            if (bucket.Count < MAX_PER_BUCKET)
                bucket.Add(arguments);
        }

        public void Clear()
        {
            for (int i = 0; i <= MAX_SIZE; i++)
                _buckets[i].Clear();
        }

        #endregion
    }
}

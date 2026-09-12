using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public static class ArgumentBag
    {
        #region Statements

        public static readonly object[] EMPTY = new object[0];

        #endregion

        #region Methods

        public static bool TryTake(List<object> remaining, Type contractType, out object value)
        {
            value = null;

            if (remaining == null || remaining.Count == 0)
                return false;

            for (int i = 0; i < remaining.Count; i++)
            {
                object candidate = remaining[i];

                if (candidate == null || !contractType.IsInstanceOfType(candidate))
                    continue;

                value = candidate;
                remaining.RemoveAt(i);

                return true;
            }

            return false;
        }

        public static List<object> Copy(IReadOnlyList<object> arguments)
        {
            if (arguments == null || arguments.Count == 0)
                return null;

            List<object> copy = new List<object>(arguments.Count);

            for (int i = 0; i < arguments.Count; i++)
                copy.Add(arguments[i]);

            return copy;
        }

        #endregion
    }
}

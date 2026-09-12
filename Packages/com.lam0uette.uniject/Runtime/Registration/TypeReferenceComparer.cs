using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LaM0uette.UniJect
{
    public sealed class TypeReferenceComparer : IEqualityComparer<Type>
    {
        #region Statements

        public static readonly TypeReferenceComparer Instance = new TypeReferenceComparer();

        private TypeReferenceComparer()
        {
        }

        #endregion

        #region Methods

        public bool Equals(Type x, Type y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(Type obj)
        {
            return obj == null ? 0 : RuntimeHelpers.GetHashCode(obj);
        }

        #endregion
    }
}

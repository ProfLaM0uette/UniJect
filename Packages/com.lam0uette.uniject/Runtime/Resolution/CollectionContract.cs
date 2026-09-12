using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal static class CollectionContract
    {
        public static bool TryGetElementType(Type contractType, out Type elementType)
        {
            if (contractType.IsArray && contractType.GetArrayRank() == 1)
            {
                elementType = contractType.GetElementType();
                return true;
            }

            if (!contractType.IsGenericType)
            {
                elementType = null;
                return false;
            }

            Type definition = contractType.GetGenericTypeDefinition();

            if (definition == typeof(IEnumerable<>) || definition == typeof(IReadOnlyList<>) ||
                definition == typeof(IReadOnlyCollection<>) || definition == typeof(IList<>) ||
                definition == typeof(ICollection<>) || definition == typeof(List<>))
            {
                elementType = contractType.GetGenericArguments()[0];
                return true;
            }

            elementType = null;
            return false;
        }

        public static object Materialize(Type contractType, Type elementType, IReadOnlyList<object> items)
        {
            Array array = Array.CreateInstance(elementType, items.Count);

            for (int i = 0; i < items.Count; i++)
                array.SetValue(items[i], i);

            if (!IsConcreteList(contractType))
                return array;

            Type listType = typeof(List<>).MakeGenericType(elementType);
            return Activator.CreateInstance(listType, array);
        }

        public static bool IsConcreteList(Type contractType)
        {
            return contractType.IsGenericType && contractType.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}

using System;
using System.Text;

namespace LaM0uette.UniJect
{
    internal static class ConditionTypeMatch
    {
        public static bool Matches(Type[] types, Type consumerType)
        {
            if (consumerType == null)
                return false;

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsAssignableFrom(consumerType))
                    return true;
            }

            return false;
        }

        public static string Describe(Type[] types)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < types.Length; i++)
            {
                if (i > 0)
                    builder.Append(", ");

                builder.Append(types[i].Name);
            }

            return builder.ToString();
        }
    }
}

using System;
using System.Reflection;

namespace LaM0uette.UniJect
{
    internal static class FactoryParameterCheck
    {
        public static void Assert(Type productType, Type[] parameterTypes, IActivatorSource productSource)
        {
            if (parameterTypes == null || parameterTypes.Length == 0)
                return;

            if (productType == null || productType.IsAbstract || productType.IsInterface)
                return;

            if (productSource != null && !(productSource is ConstructorActivatorSource))
                return;

            if (ConstructorSelector.IsEngineConstructed(productType))
                return;

            ConstructorInfo[] constructors = productType.GetConstructors(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < constructors.Length; i++)
            {
                if (Accepts(constructors[i], parameterTypes))
                    return;
            }

            throw new InvalidBindingException(
                IssueCode.UJ013,
                productType,
                BindingOrigin.Unknown,
                "the factory declares (" + Describe(parameterTypes) + ") but no constructor of " +
                productType.Name + " accepts those parameters");
        }


        private static bool Accepts(ConstructorInfo constructor, Type[] parameterTypes)
        {
            ParameterInfo[] parameters = constructor.GetParameters();

            for (int i = 0; i < parameterTypes.Length; i++)
            {
                if (!HasAssignable(parameters, parameterTypes[i]))
                    return false;
            }

            return true;
        }

        private static bool HasAssignable(ParameterInfo[] parameters, Type parameterType)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.IsAssignableFrom(parameterType))
                    return true;
            }

            return false;
        }

        private static string Describe(Type[] parameterTypes)
        {
            string text = string.Empty;

            for (int i = 0; i < parameterTypes.Length; i++)
            {
                if (i > 0)
                    text += ", ";

                text += parameterTypes[i].Name;
            }

            return text;
        }
    }
}

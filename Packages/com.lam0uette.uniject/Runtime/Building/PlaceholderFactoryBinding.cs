using System;

namespace LaM0uette.UniJect
{
    internal static class PlaceholderFactoryBinding
    {
        public static PlaceholderFactoryBinder<TFactory, TProduct> Declare<TFactory, TProduct>(
            IContainerBuilder builder,
            Type contractType,
            string sourceFile,
            int sourceLine)
        {
            PlaceholderFactoryActivatorSource source = new PlaceholderFactoryActivatorSource(
                typeof(TProduct),
                ParameterTypes(typeof(TFactory)));

            BindingDraft draft = FactoryDraft.Declare(
                builder,
                contractType,
                typeof(TFactory),
                sourceFile,
                sourceLine);

            draft.Source = source;
            draft.SetLifetime(Lifetime.Singleton);

            return new PlaceholderFactoryBinder<TFactory, TProduct>(draft, source);
        }


        private static Type[] ParameterTypes(Type factoryType)
        {
            for (Type current = factoryType; current != null; current = current.BaseType)
            {
                if (!current.IsGenericType)
                    continue;

                if (current.GetGenericTypeDefinition().Name.StartsWith("PlaceholderFactory", StringComparison.Ordinal))
                {
                    Type[] arguments = current.GetGenericArguments();

                    if (arguments.Length <= 1)
                        return null;

                    Type[] parameters = new Type[arguments.Length - 1];

                    for (int i = 1; i < arguments.Length; i++)
                        parameters[i - 1] = arguments[i];

                    return parameters;
                }
            }

            return null;
        }
    }
}

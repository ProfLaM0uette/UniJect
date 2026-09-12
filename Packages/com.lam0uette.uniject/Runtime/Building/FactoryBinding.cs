using System;

namespace LaM0uette.UniJect
{
    internal static class FactoryBinding
    {
        public static FactoryBinder<TProduct> Declare<TProduct>(
            IContainerBuilder builder,
            Type contractType,
            Func<IProductFactory, object> adapter,
            string sourceFile,
            int sourceLine)
        {
            FactoryActivatorSource source = new FactoryActivatorSource(
                typeof(TProduct),
                adapter,
                ParameterTypes(contractType));

            BindingDraft draft = FactoryDraft.Declare(builder, contractType, contractType, sourceFile, sourceLine);
            draft.Source = source;
            draft.SetLifetime(Lifetime.Singleton);

            return new FactoryBinder<TProduct>(draft, source);
        }

        public static FactoryBinder<TProduct> DeclareTo<TProduct>(
            IContainerBuilder builder,
            Type contractType,
            Type factoryType,
            string sourceFile,
            int sourceLine)
        {
            FactoryActivatorSource source = new FactoryActivatorSource(typeof(TProduct), null, null);

            BindingDraft draft = FactoryDraft.Declare(builder, contractType, factoryType, sourceFile, sourceLine);
            draft.SetLifetime(Lifetime.Singleton);

            return new FactoryBinder<TProduct>(draft, source);
        }


        private static Type[] ParameterTypes(Type contractType)
        {
            if (!contractType.IsGenericType)
                return null;

            Type[] arguments = contractType.GetGenericArguments();

            if (arguments.Length <= 1)
                return null;

            Type[] parameters = new Type[arguments.Length - 1];

            for (int i = 1; i < arguments.Length; i++)
                parameters[i - 1] = arguments[i];

            return parameters;
        }
    }
}

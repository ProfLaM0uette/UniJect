using System;
using System.Runtime.CompilerServices;

namespace LaM0uette.UniJect
{
    public static class ContainerBuilderBindExtensions
    {
        public static Binder<TContract, TContract> Bind<TContract>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            BindingDraft draft = Declare(builder, typeof(TContract), typeof(TContract), sourceFile, sourceLine);
            return new Binder<TContract, TContract>(draft);
        }

        public static Binder<TContract, TConcrete> Bind<TContract, TConcrete>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TConcrete : TContract
        {
            BindingDraft draft = Declare(builder, typeof(TContract), typeof(TConcrete), sourceFile, sourceLine);
            return new Binder<TContract, TConcrete>(draft);
        }

        public static Binder<TConcrete, TConcrete> BindInstance<TConcrete>(
            this IContainerBuilder builder,
            TConcrete instance,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            BindingDraft draft = Declare(builder, typeof(TConcrete), typeof(TConcrete), sourceFile, sourceLine);
            draft.SetSource(new InstanceActivatorSource(instance));
            draft.SetLifetime(Lifetime.Singleton);

            return new Binder<TConcrete, TConcrete>(draft);
        }


        private static BindingDraft Declare(
            IContainerBuilder builder,
            Type contractType,
            Type concreteType,
            string sourceFile,
            int sourceLine)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            BindingDraft draft = new BindingDraft
            {
                ConcreteType = concreteType,
                Origin = Origin(sourceFile, sourceLine)
            };

            draft.ContractTypes.Add(contractType);
            Sink(builder).AddDraft(draft);

            return draft;
        }

        private static BindingOrigin Origin(string sourceFile, int sourceLine)
        {
#if UNIJECT_NO_SOURCE_INFO
            return BindingOrigin.Unknown;
#else
            return new BindingOrigin(sourceFile, sourceLine);
#endif
        }

        private static ContainerBuilder Sink(IContainerBuilder builder)
        {
            if (builder is ContainerBuilder concrete)
                return concrete;

            throw new InvalidOperationException(
                "UniJect: Bind<T>() needs a ContainerBuilder, but the builder is " + builder.GetType().Name +
                ". Declare bindings on the builder the container hands you.");
        }
    }
}

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


        public static Binder<TConcrete, TConcrete> BindInterfacesTo<TConcrete>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            return DeclareInterfaces<TConcrete>(builder, false, sourceFile, sourceLine);
        }

        public static Binder<TConcrete, TConcrete> BindInterfacesAndSelfTo<TConcrete>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            return DeclareInterfaces<TConcrete>(builder, true, sourceFile, sourceLine);
        }


        public static void Install(this IContainerBuilder builder, IInstaller installer)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (installer == null)
                throw new ArgumentNullException(nameof(installer));

            installer.Install(builder);
        }


        private static Binder<TConcrete, TConcrete> DeclareInterfaces<TConcrete>(
            IContainerBuilder builder,
            bool includeSelf,
            string sourceFile,
            int sourceLine)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Type concreteType = typeof(TConcrete);
            Type[] interfaces = concreteType.GetInterfaces();

            if (interfaces.Length == 0 && !includeSelf)
            {
                throw new InvalidBindingException(
                    IssueCode.UJ003,
                    concreteType,
                    Origin(sourceFile, sourceLine),
                    concreteType.Name + " implements no interface, so BindInterfacesTo has nothing to bind");
            }

            BindingDraft draft = new BindingDraft
            {
                ConcreteType = concreteType,
                Origin = Origin(sourceFile, sourceLine)
            };

            for (int i = 0; i < interfaces.Length; i++)
                draft.ContractTypes.Add(interfaces[i]);

            if (includeSelf)
                draft.ContractTypes.Add(concreteType);

            Sink(builder).AddDraft(draft);

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

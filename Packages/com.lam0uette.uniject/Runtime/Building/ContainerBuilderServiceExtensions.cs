using System;
using System.Runtime.CompilerServices;

namespace LaM0uette.UniJect
{
    public static class ContainerBuilderServiceExtensions
    {
        public static IContainerBuilder AddSingleton<TService, TImplementation>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TImplementation : TService
        {
            builder.Bind<TService, TImplementation>(sourceFile, sourceLine).AsSingleton();
            return builder;
        }

        public static IContainerBuilder AddSingleton<TService>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            builder.Bind<TService>(sourceFile, sourceLine).AsSingleton();
            return builder;
        }

        public static IContainerBuilder AddSingleton<TService>(
            this IContainerBuilder builder,
            TService instance,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            builder.BindInstance(instance, sourceFile, sourceLine);
            return builder;
        }

        public static IContainerBuilder AddSingleton<TService>(
            this IContainerBuilder builder,
            Func<IResolver, TService> factory,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            builder.Bind<TService>(sourceFile, sourceLine).FromMethod(factory).AsSingleton();
            return builder;
        }


        public static IContainerBuilder AddScoped<TService, TImplementation>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TImplementation : TService
        {
            builder.Bind<TService, TImplementation>(sourceFile, sourceLine).AsScoped();
            return builder;
        }

        public static IContainerBuilder AddScoped<TService>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            builder.Bind<TService>(sourceFile, sourceLine).AsScoped();
            return builder;
        }

        public static IContainerBuilder AddScoped<TService>(
            this IContainerBuilder builder,
            Func<IResolver, TService> factory,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            builder.Bind<TService>(sourceFile, sourceLine).FromMethod(factory).AsScoped();
            return builder;
        }


        public static IContainerBuilder AddTransient<TService, TImplementation>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TImplementation : TService
        {
            builder.Bind<TService, TImplementation>(sourceFile, sourceLine).AsTransient();
            return builder;
        }

        public static IContainerBuilder AddTransient<TService>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            builder.Bind<TService>(sourceFile, sourceLine).AsTransient();
            return builder;
        }

        public static IContainerBuilder AddTransient<TService>(
            this IContainerBuilder builder,
            Func<IResolver, TService> factory,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            builder.Bind<TService>(sourceFile, sourceLine).FromMethod(factory).AsTransient();
            return builder;
        }


        public static IContainerBuilder TryAddSingleton<TService, TImplementation>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TImplementation : TService
        {
            builder.Bind<TService, TImplementation>(sourceFile, sourceLine).IfNotBound().AsSingleton();
            return builder;
        }

        public static IContainerBuilder Replace<TService, TImplementation>(
            this IContainerBuilder builder,
            Lifetime lifetime,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TImplementation : TService
        {
            if (builder is ContainerBuilder concrete)
                concrete.RemoveDeclared(typeof(TService), null);

            Binder<TService, TImplementation> binder = builder.Bind<TService, TImplementation>(sourceFile, sourceLine);
            binder.Draft.SetLifetime(lifetime);

            return builder;
        }
    }
}

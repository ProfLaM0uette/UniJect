using System.Runtime.CompilerServices;

namespace LaM0uette.UniJect
{
    [System.CodeDom.Compiler.GeneratedCode("UniJect.ArityGenerator", "1.0")]
    public static class ContainerBuilderPlaceholderFactoryExtensions
    {
        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactory<TFactory, TProduct>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct>
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactory<TFactory, TProduct, P1>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct, P1>
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactory<TFactory, TProduct, P1, P2>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct, P1, P2>
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactory<TFactory, TProduct, P1, P2, P3>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct, P1, P2, P3>
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactory<TFactory, TProduct, P1, P2, P3, P4>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct, P1, P2, P3, P4>
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactory<TFactory, TProduct, P1, P2, P3, P4, P5>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct, P1, P2, P3, P4, P5>
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactory<TFactory, TProduct, P1, P2, P3, P4, P5, P6>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct, P1, P2, P3, P4, P5, P6>
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactoryTo<TInterface, TFactory, TProduct>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct>, TInterface
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TInterface),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactoryTo<TInterface, TFactory, TProduct, P1>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct, P1>, TInterface
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TInterface),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactoryTo<TInterface, TFactory, TProduct, P1, P2>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct, P1, P2>, TInterface
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TInterface),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactoryTo<TInterface, TFactory, TProduct, P1, P2, P3>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct, P1, P2, P3>, TInterface
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TInterface),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactoryTo<TInterface, TFactory, TProduct, P1, P2, P3, P4>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct, P1, P2, P3, P4>, TInterface
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TInterface),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactoryTo<TInterface, TFactory, TProduct, P1, P2, P3, P4, P5>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct, P1, P2, P3, P4, P5>, TInterface
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TInterface),
                sourceFile,
                sourceLine);
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> BindPlaceholderFactoryTo<TInterface, TFactory, TProduct, P1, P2, P3, P4, P5, P6>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : PlaceholderFactory<TProduct, P1, P2, P3, P4, P5, P6>, TInterface
        {
            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(
                builder,
                typeof(TInterface),
                sourceFile,
                sourceLine);
        }
    }
}

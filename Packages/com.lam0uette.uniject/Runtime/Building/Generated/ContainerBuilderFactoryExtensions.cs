using System.Runtime.CompilerServices;

namespace LaM0uette.UniJect
{
    [System.CodeDom.Compiler.GeneratedCode("UniJect.ArityGenerator", "1.0")]
    public static class ContainerBuilderFactoryExtensions
    {
        public static FactoryBinder<TProduct> BindFactory<TProduct>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            return FactoryBinding.Declare<TProduct>(
                builder,
                typeof(IFactory<TProduct>),
                productFactory => new ProductFactoryAdapter<TProduct>(productFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactory<TProduct, P1>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            return FactoryBinding.Declare<TProduct>(
                builder,
                typeof(IFactory<TProduct, P1>),
                productFactory => new ProductFactoryAdapter<TProduct, P1>(productFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactory<TProduct, P1, P2>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            return FactoryBinding.Declare<TProduct>(
                builder,
                typeof(IFactory<TProduct, P1, P2>),
                productFactory => new ProductFactoryAdapter<TProduct, P1, P2>(productFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactory<TProduct, P1, P2, P3>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            return FactoryBinding.Declare<TProduct>(
                builder,
                typeof(IFactory<TProduct, P1, P2, P3>),
                productFactory => new ProductFactoryAdapter<TProduct, P1, P2, P3>(productFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactory<TProduct, P1, P2, P3, P4>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            return FactoryBinding.Declare<TProduct>(
                builder,
                typeof(IFactory<TProduct, P1, P2, P3, P4>),
                productFactory => new ProductFactoryAdapter<TProduct, P1, P2, P3, P4>(productFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactory<TProduct, P1, P2, P3, P4, P5>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            return FactoryBinding.Declare<TProduct>(
                builder,
                typeof(IFactory<TProduct, P1, P2, P3, P4, P5>),
                productFactory => new ProductFactoryAdapter<TProduct, P1, P2, P3, P4, P5>(productFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactory<TProduct, P1, P2, P3, P4, P5, P6>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
        {
            return FactoryBinding.Declare<TProduct>(
                builder,
                typeof(IFactory<TProduct, P1, P2, P3, P4, P5, P6>),
                productFactory => new ProductFactoryAdapter<TProduct, P1, P2, P3, P4, P5, P6>(productFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactoryTo<TInterface, TFactory, TProduct>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : TInterface, IFactory<TProduct>
        {
            return FactoryBinding.DeclareTo<TProduct>(
                builder,
                typeof(TInterface),
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactoryTo<TInterface, TFactory, TProduct, P1>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : TInterface, IFactory<TProduct, P1>
        {
            return FactoryBinding.DeclareTo<TProduct>(
                builder,
                typeof(TInterface),
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactoryTo<TInterface, TFactory, TProduct, P1, P2>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : TInterface, IFactory<TProduct, P1, P2>
        {
            return FactoryBinding.DeclareTo<TProduct>(
                builder,
                typeof(TInterface),
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactoryTo<TInterface, TFactory, TProduct, P1, P2, P3>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : TInterface, IFactory<TProduct, P1, P2, P3>
        {
            return FactoryBinding.DeclareTo<TProduct>(
                builder,
                typeof(TInterface),
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactoryTo<TInterface, TFactory, TProduct, P1, P2, P3, P4>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : TInterface, IFactory<TProduct, P1, P2, P3, P4>
        {
            return FactoryBinding.DeclareTo<TProduct>(
                builder,
                typeof(TInterface),
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactoryTo<TInterface, TFactory, TProduct, P1, P2, P3, P4, P5>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : TInterface, IFactory<TProduct, P1, P2, P3, P4, P5>
        {
            return FactoryBinding.DeclareTo<TProduct>(
                builder,
                typeof(TInterface),
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }

        public static FactoryBinder<TProduct> BindFactoryTo<TInterface, TFactory, TProduct, P1, P2, P3, P4, P5, P6>(
            this IContainerBuilder builder,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int sourceLine = 0)
            where TFactory : TInterface, IFactory<TProduct, P1, P2, P3, P4, P5, P6>
        {
            return FactoryBinding.DeclareTo<TProduct>(
                builder,
                typeof(TInterface),
                typeof(TFactory),
                sourceFile,
                sourceLine);
        }
    }
}

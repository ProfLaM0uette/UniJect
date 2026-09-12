using System;
using System.Text;

namespace UniJect.ArityGenerator
{
    internal static class BindExtensionWriter
    {
        private static readonly string[] USINGS = { "System.Runtime.CompilerServices" };

        public static void WriteAll(string directory)
        {
            GeneratorConventions.Write(
                directory,
                "ContainerBuilderFactoryExtensions.cs",
                FactoryExtensions(),
                USINGS);

            GeneratorConventions.Write(
                directory,
                "ContainerBuilderPlaceholderFactoryExtensions.cs",
                PlaceholderExtensions(),
                USINGS);
        }


        private static string FactoryExtensions()
        {
            StringBuilder builder = new StringBuilder();
            Line(builder, GeneratorConventions.GENERATED_ATTRIBUTE);
            Line(builder, "    public static class ContainerBuilderFactoryExtensions");
            Line(builder, "    {");

            for (int arity = 0; arity <= GeneratorConventions.FACTORY_MAX_ARITY; arity++)
            {
                if (arity > 0)
                    Line(builder, "");

                BindFactory(builder, arity);
            }

            for (int arity = 0; arity <= GeneratorConventions.FACTORY_MAX_ARITY; arity++)
            {
                Line(builder, "");
                BindFactoryTo(builder, arity);
            }

            Line(builder, "    }");

            return builder.ToString();
        }

        private static string PlaceholderExtensions()
        {
            StringBuilder builder = new StringBuilder();
            Line(builder, GeneratorConventions.GENERATED_ATTRIBUTE);
            Line(builder, "    public static class ContainerBuilderPlaceholderFactoryExtensions");
            Line(builder, "    {");

            for (int arity = 0; arity <= GeneratorConventions.FACTORY_MAX_ARITY; arity++)
            {
                if (arity > 0)
                    Line(builder, "");

                BindPlaceholder(builder, arity, false);
            }

            for (int arity = 0; arity <= GeneratorConventions.FACTORY_MAX_ARITY; arity++)
            {
                Line(builder, "");
                BindPlaceholder(builder, arity, true);
            }

            Line(builder, "    }");

            return builder.ToString();
        }

        private static void BindFactory(StringBuilder builder, int arity)
        {
            string products = ProductGenerics(arity);

            Line(builder, "        public static FactoryBinder<TProduct> BindFactory<" + products + ">(");
            Line(builder, "            this IContainerBuilder builder,");
            Line(builder, "            [CallerFilePath] string sourceFile = null,");
            Line(builder, "            [CallerLineNumber] int sourceLine = 0)");
            Line(builder, "        {");
            Line(builder, "            return FactoryBinding.Declare<TProduct>(");
            Line(builder, "                builder,");
            Line(builder, "                typeof(IFactory<" + products + ">),");
            Line(builder, "                productFactory => new ProductFactoryAdapter<" + products +
                          ">(productFactory),");
            Line(builder, "                sourceFile,");
            Line(builder, "                sourceLine);");
            Line(builder, "        }");
        }

        private static void BindFactoryTo(StringBuilder builder, int arity)
        {
            string products = ProductGenerics(arity);

            Line(builder, "        public static FactoryBinder<TProduct> BindFactoryTo<TInterface, TFactory, " +
                          products + ">(");
            Line(builder, "            this IContainerBuilder builder,");
            Line(builder, "            [CallerFilePath] string sourceFile = null,");
            Line(builder, "            [CallerLineNumber] int sourceLine = 0)");
            Line(builder, "            where TFactory : TInterface, IFactory<" + products + ">");
            Line(builder, "        {");
            Line(builder, "            return FactoryBinding.DeclareTo<TProduct>(");
            Line(builder, "                builder,");
            Line(builder, "                typeof(TInterface),");
            Line(builder, "                typeof(TFactory),");
            Line(builder, "                sourceFile,");
            Line(builder, "                sourceLine);");
            Line(builder, "        }");
        }

        private static void BindPlaceholder(StringBuilder builder, int arity, bool toInterface)
        {
            string products = ProductGenerics(arity);
            string name = toInterface ? "BindPlaceholderFactoryTo" : "BindPlaceholderFactory";
            string generics = toInterface ? "TInterface, TFactory, " + products : "TFactory, " + products;
            string constraint = toInterface
                ? "            where TFactory : PlaceholderFactory<" + products + ">, TInterface"
                : "            where TFactory : PlaceholderFactory<" + products + ">";
            string contract = toInterface ? "typeof(TInterface)" : "typeof(TFactory)";

            Line(builder, "        public static PlaceholderFactoryBinder<TFactory, TProduct> " + name + "<" +
                          generics + ">(");
            Line(builder, "            this IContainerBuilder builder,");
            Line(builder, "            [CallerFilePath] string sourceFile = null,");
            Line(builder, "            [CallerLineNumber] int sourceLine = 0)");
            Line(builder, constraint);
            Line(builder, "        {");
            Line(builder, "            return PlaceholderFactoryBinding.Declare<TFactory, TProduct>(");
            Line(builder, "                builder,");
            Line(builder, "                " + contract + ",");
            Line(builder, "                sourceFile,");
            Line(builder, "                sourceLine);");
            Line(builder, "        }");
        }

        private static string ProductGenerics(int arity)
        {
            return arity == 0 ? "TProduct" : "TProduct, " + GeneratorConventions.TypeList("P", arity, "");
        }

        private static void Line(StringBuilder builder, string text)
        {
            builder.Append(text).Append(Environment.NewLine);
        }
    }
}

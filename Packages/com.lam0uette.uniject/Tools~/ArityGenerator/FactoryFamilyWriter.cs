using System;
using System.Text;

namespace UniJect.ArityGenerator
{
    internal static class FactoryFamilyWriter
    {
        public static void WriteAll(string directory)
        {
            for (int arity = 0; arity <= GeneratorConventions.FACTORY_MAX_ARITY; arity++)
            {
                string suffix = GeneratorConventions.Suffix(arity);

                GeneratorConventions.Write(directory, "IFactory" + suffix + ".cs", Interface(arity));
                GeneratorConventions.Write(directory, "PlaceholderFactory" + suffix + ".cs", Placeholder(arity));
                GeneratorConventions.Write(directory, "ProductFactoryAdapter" + suffix + ".cs", Adapter(arity));
            }
        }


        private static string Interface(int arity)
        {
            string generics = arity == 0
                ? "out TProduct"
                : "out TProduct, " + GeneratorConventions.TypeList("P", arity, "in");

            StringBuilder builder = new StringBuilder();
            Line(builder, GeneratorConventions.GENERATED_ATTRIBUTE);
            Line(builder, "    public interface IFactory<" + generics + ">");
            Line(builder, "    {");
            Line(builder, "        TProduct Create(" + GeneratorConventions.ParameterList("P", "p", arity) + ");");
            Line(builder, "    }");

            return builder.ToString();
        }

        private static string Placeholder(int arity)
        {
            string generics = arity == 0 ? "TProduct" : "TProduct, " + GeneratorConventions.TypeList("P", arity, "");

            StringBuilder builder = new StringBuilder();
            Line(builder, GeneratorConventions.GENERATED_ATTRIBUTE);
            Line(builder, "    public abstract class PlaceholderFactory<" + generics +
                          "> : PlaceholderFactoryBase<TProduct>, IFactory<" + generics + ">");
            Line(builder, "    {");
            Line(builder, "        public virtual TProduct Create(" +
                          GeneratorConventions.ParameterList("P", "p", arity) + ")");
            Line(builder, "        {");
            Line(builder, "            return (TProduct)ProductFactory.Create(" + ArgumentArray(arity) + ");");
            Line(builder, "        }");
            Line(builder, "    }");

            return builder.ToString();
        }

        private static string Adapter(int arity)
        {
            string generics = arity == 0 ? "TProduct" : "TProduct, " + GeneratorConventions.TypeList("P", arity, "");

            StringBuilder builder = new StringBuilder();
            Line(builder, GeneratorConventions.GENERATED_ATTRIBUTE);
            Line(builder, "    internal sealed class ProductFactoryAdapter<" + generics + "> : IFactory<" +
                          generics + ">");
            Line(builder, "    {");
            Line(builder, "        private readonly IProductFactory _productFactory;");
            Line(builder, "");
            Line(builder, "        public ProductFactoryAdapter(IProductFactory productFactory)");
            Line(builder, "        {");
            Line(builder, "            _productFactory = productFactory;");
            Line(builder, "        }");
            Line(builder, "");
            Line(builder, "        public TProduct Create(" +
                          GeneratorConventions.ParameterList("P", "p", arity) + ")");
            Line(builder, "        {");
            Line(builder, "            return (TProduct)_productFactory.Create(" + ArgumentArray(arity) + ");");
            Line(builder, "        }");
            Line(builder, "    }");

            return builder.ToString();
        }

        private static string ArgumentArray(int arity)
        {
            if (arity == 0)
                return "ArgumentBag.EMPTY";

            return "new object[] { " + GeneratorConventions.ArgumentList("p", arity) + " }";
        }

        private static void Line(StringBuilder builder, string text)
        {
            builder.Append(text).Append(Environment.NewLine);
        }
    }
}

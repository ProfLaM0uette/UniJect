using System;
using System.Text;

namespace UniJect.ArityGenerator
{
    internal static class InjectableFamilyWriter
    {
        public static void WriteAll(string directory)
        {
            for (int arity = 1; arity <= GeneratorConventions.INJECTABLE_MAX_ARITY; arity++)
                GeneratorConventions.Write(directory, "IInjectable" + arity + ".cs", Interface(arity));

            GeneratorConventions.Write(directory, "InjectableDefinitions.cs", Definitions());
        }


        private static string Interface(int arity)
        {
            StringBuilder builder = new StringBuilder();
            Line(builder, GeneratorConventions.GENERATED_ATTRIBUTE);
            Line(builder, "    public interface IInjectable<" + GeneratorConventions.TypeList("T", arity, "in") + ">");
            Line(builder, "    {");
            Line(builder, "        void Inject(" + GeneratorConventions.ParameterList("T", "t", arity) + ");");
            Line(builder, "    }");

            return builder.ToString();
        }

        private static string Definitions()
        {
            StringBuilder builder = new StringBuilder();
            Line(builder, GeneratorConventions.GENERATED_ATTRIBUTE);
            Line(builder, "    internal static class InjectableDefinitions");
            Line(builder, "    {");
            Line(builder, "        public static readonly System.Type[] ALL =");
            Line(builder, "        {");

            for (int arity = 1; arity <= GeneratorConventions.INJECTABLE_MAX_ARITY; arity++)
            {
                string commas = new string(',', arity - 1);
                string tail = arity < GeneratorConventions.INJECTABLE_MAX_ARITY ? "," : "";

                Line(builder, "            typeof(IInjectable<" + commas + ">)" + tail);
            }

            Line(builder, "        };");
            Line(builder, "    }");

            return builder.ToString();
        }

        private static void Line(StringBuilder builder, string text)
        {
            builder.Append(text).Append(Environment.NewLine);
        }
    }
}

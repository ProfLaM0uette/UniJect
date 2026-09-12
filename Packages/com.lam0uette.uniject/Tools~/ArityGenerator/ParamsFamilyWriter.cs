using System;
using System.Text;

namespace UniJect.ArityGenerator
{
    internal static class ParamsFamilyWriter
    {
        public static void WriteAll(string directory)
        {
            for (int arity = 2; arity <= GeneratorConventions.PARAMS_MAX_ARITY; arity++)
            {
                GeneratorConventions.Write(directory, "IParams" + arity + ".cs", Interface(arity));
                GeneratorConventions.Write(directory, "Params" + arity + ".cs", Implementation(arity));
            }
        }


        private static string Interface(int arity)
        {
            StringBuilder builder = new StringBuilder();
            Line(builder, GeneratorConventions.GENERATED_ATTRIBUTE);
            Line(builder, "    public interface IParams<" + GeneratorConventions.TypeList("T", arity, "out") + ">");
            Line(builder, "    {");

            for (int i = 1; i <= arity; i++)
                Line(builder, "        T" + i + " P" + i + " { get; }");

            Line(builder, "    }");

            return builder.ToString();
        }

        private static string Implementation(int arity)
        {
            string generics = GeneratorConventions.TypeList("T", arity, "");

            StringBuilder builder = new StringBuilder();
            Line(builder, GeneratorConventions.GENERATED_ATTRIBUTE);
            Line(builder, "    public class Params<" + generics + "> : IParams<" + generics + ">");
            Line(builder, "    {");

            for (int i = 1; i <= arity; i++)
                Line(builder, "        public T" + i + " P" + i + " { get; }");

            Line(builder, "");
            Line(builder, "        public Params(" + GeneratorConventions.ParameterList("T", "p", arity) + ")");
            Line(builder, "        {");

            for (int i = 1; i <= arity; i++)
                Line(builder, "            P" + i + " = p" + i + ";");

            Line(builder, "        }");
            Line(builder, "    }");

            return builder.ToString();
        }

        private static void Line(StringBuilder builder, string text)
        {
            builder.Append(text).Append(Environment.NewLine);
        }
    }
}

using System;
using System.IO;
using System.Text;

namespace UniJect.ArityGenerator
{
    internal static class GeneratorConventions
    {
        public const int FACTORY_MAX_ARITY = 6;
        public const int PARAMS_MAX_ARITY = 6;
        public const int INJECTABLE_MAX_ARITY = 8;

        public const string NAMESPACE = "LaM0uette.UniJect";
        public const string GENERATED_ATTRIBUTE =
            "    [System.CodeDom.Compiler.GeneratedCode(\"UniJect.ArityGenerator\", \"1.0\")]";

        public static void Write(string directory, string fileName, string body)
        {
            Write(directory, fileName, body, null);
        }

        public static void Write(string directory, string fileName, string body, string[] usings)
        {
            Directory.CreateDirectory(directory);

            StringBuilder builder = new StringBuilder();

            if (usings != null)
            {
                for (int i = 0; i < usings.Length; i++)
                    builder.Append("using ").Append(usings[i]).Append(";").Append(Environment.NewLine);

                builder.Append(Environment.NewLine);
            }

            builder.Append("namespace ").Append(NAMESPACE).Append(Environment.NewLine);
            builder.Append("{").Append(Environment.NewLine);
            builder.Append(body);
            builder.Append("}").Append(Environment.NewLine);

            string path = Path.Combine(directory, fileName);
            string content = builder.ToString().Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

            File.WriteAllText(path, content, new UTF8Encoding(false));
        }

        public static string TypeList(string prefix, int arity, string variance)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 1; i <= arity; i++)
            {
                if (i > 1)
                    builder.Append(", ");

                if (variance.Length > 0)
                    builder.Append(variance).Append(' ');

                builder.Append(prefix).Append(i);
            }

            return builder.ToString();
        }

        public static string ParameterList(string typePrefix, string namePrefix, int arity)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 1; i <= arity; i++)
            {
                if (i > 1)
                    builder.Append(", ");

                builder.Append(typePrefix).Append(i).Append(' ').Append(namePrefix).Append(i);
            }

            return builder.ToString();
        }

        public static string ArgumentList(string namePrefix, int arity)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 1; i <= arity; i++)
            {
                if (i > 1)
                    builder.Append(", ");

                builder.Append(namePrefix).Append(i);
            }

            return builder.ToString();
        }

        public static string Suffix(int arity)
        {
            return arity == 0 ? string.Empty : arity.ToString();
        }
    }
}

using System.Collections.Generic;
using System.Text;

namespace LaM0uette.UniJect
{
    internal static class ContainerDiagnostics
    {
        public static string Describe(DIContainer container)
        {
            if (container == null)
                return null;

            return container.Options.Description;
        }

        public static string DescribeInstallers(DIContainer container)
        {
            IReadOnlyList<string> names = container?.Options.InstallerNames;

            if (names == null || names.Count == 0)
                return null;

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < names.Count; i++)
            {
                if (i > 0)
                    builder.Append(", ");

                builder.Append(names[i]);
            }

            return builder.ToString();
        }
    }
}

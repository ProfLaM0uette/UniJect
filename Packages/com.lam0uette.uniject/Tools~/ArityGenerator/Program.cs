using System;
using System.IO;

namespace UniJect.ArityGenerator
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            string packageRoot = args.Length > 0 ? args[0] : DefaultPackageRoot();

            if (!Directory.Exists(Path.Combine(packageRoot, "Runtime")))
            {
                Console.Error.WriteLine("UniJect: '" + packageRoot + "' is not the package root.");
                return 1;
            }

            string factories = Path.Combine(packageRoot, "Runtime", "Factories", "Generated");
            string injectable = Path.Combine(packageRoot, "Runtime", "Injectable", "Generated");
            string building = Path.Combine(packageRoot, "Runtime", "Building", "Generated");

            FactoryFamilyWriter.WriteAll(factories);
            ParamsFamilyWriter.WriteAll(factories);
            InjectableFamilyWriter.WriteAll(injectable);
            BindExtensionWriter.WriteAll(building);

            Console.WriteLine("UniJect: arity families written under " + packageRoot + ".");
            return 0;
        }

        private static string DefaultPackageRoot()
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        }
    }
}

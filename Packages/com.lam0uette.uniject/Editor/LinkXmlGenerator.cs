using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;

namespace LaM0uette.UniJect
{
    internal sealed class LinkXmlGenerator : IUnityLinkerProcessor
    {
        #region Statements

        private const string FILE_NAME = "UniJect.generated.link.xml";

        public int callbackOrder
        {
            get { return 0; }
        }

        #endregion

        #region Methods

        public string GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
        {
            Dictionary<Assembly, HashSet<Type>> preserved = CollectInjectedTypes();
            string path = Path.Combine(Path.GetTempPath(), FILE_NAME);

            File.WriteAllText(path, Render(preserved), new UTF8Encoding(false));

            return path;
        }


        private static Dictionary<Assembly, HashSet<Type>> CollectInjectedTypes()
        {
            Dictionary<Assembly, HashSet<Type>> preserved = new Dictionary<Assembly, HashSet<Type>>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types;

                try
                {
                    types = assemblies[i].GetTypes();
                }
                catch (ReflectionTypeLoadException exception)
                {
                    types = exception.Types;
                }

                for (int j = 0; j < types.Length; j++)
                {
                    Type type = types[j];

                    if (type == null || !HasInjectionSite(type))
                        continue;

                    if (!preserved.TryGetValue(assemblies[i], out HashSet<Type> bucket))
                    {
                        bucket = new HashSet<Type>();
                        preserved[assemblies[i]] = bucket;
                    }

                    bucket.Add(type);
                }
            }

            return preserved;
        }

        private static bool HasInjectionSite(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.DeclaredOnly;

            FieldInfo[] fields = type.GetFields(flags);

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].IsDefined(typeof(InjectAttributeBase), false))
                    return true;
            }

            PropertyInfo[] properties = type.GetProperties(flags);

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].IsDefined(typeof(InjectAttributeBase), false))
                    return true;
            }

            MethodInfo[] methods = type.GetMethods(flags);

            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].IsDefined(typeof(InjectAttributeBase), false))
                    return true;
            }

            ConstructorInfo[] constructors = type.GetConstructors(flags);

            for (int i = 0; i < constructors.Length; i++)
            {
                if (constructors[i].IsDefined(typeof(InjectAttributeBase), false))
                    return true;
            }

            return false;
        }

        private static string Render(Dictionary<Assembly, HashSet<Type>> preserved)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<linker>");

            foreach (KeyValuePair<Assembly, HashSet<Type>> entry in preserved)
            {
                builder.Append("  <assembly fullname=\"").Append(entry.Key.GetName().Name).AppendLine("\">");

                foreach (Type type in entry.Value)
                {
                    builder.Append("    <type fullname=\"").Append(type.FullName)
                        .AppendLine("\" preserve=\"all\" />");
                }

                builder.AppendLine("  </assembly>");
            }

            builder.AppendLine("</linker>");

            return builder.ToString();
        }

        #endregion
    }
}

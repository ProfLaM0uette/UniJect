using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using UnityEngine;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class PublicApiSnapshotTests
    {
        private const string APPROVED_FILE = "PublicApi.Core.approved.txt";

        [Test]
        public void CorePublicSurface_MatchesTheApprovedSnapshot()
        {
            string actual = Render(typeof(DIContainer).Assembly);
            string path = ApprovedPath();

            if (!File.Exists(path))
            {
                File.WriteAllText(path, actual, new UTF8Encoding(false));
                Assert.Fail("No approved snapshot yet. One was written to " + path + " — review it and re-run.");
            }

            string approved = File.ReadAllText(path).Replace("\r\n", "\n");

            Assert.That(
                actual.Replace("\r\n", "\n"),
                Is.EqualTo(approved),
                "The core public surface changed. Review the diff, then update " + APPROVED_FILE +
                " in the same commit.");
        }


        private static string Render(Assembly assembly)
        {
            List<string> blocks = new List<string>();
            Type[] types = assembly.GetTypes();

            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];

                if (!type.IsPublic && !type.IsNestedPublic)
                    continue;

                blocks.Add(RenderType(type));
            }

            blocks.Sort(StringComparer.Ordinal);

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < blocks.Count; i++)
                builder.Append(blocks[i]);

            return builder.ToString();
        }

        private static string RenderType(Type type)
        {
            MemberInfo[] members = type.GetMembers(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

            List<string> rendered = new List<string>();

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] is MethodInfo method && method.IsSpecialName)
                    continue;

                rendered.Add("    " + members[i].MemberType + " " + members[i].Name);
            }

            rendered.Sort(StringComparer.Ordinal);

            StringBuilder builder = new StringBuilder();
            builder.Append(Describe(type)).Append('\n');

            for (int i = 0; i < rendered.Count; i++)
                builder.Append(rendered[i]).Append('\n');

            return builder.ToString();
        }

        private static string Describe(Type type)
        {
            string kind = type.IsInterface ? "interface" : type.IsEnum ? "enum" : type.IsValueType ? "struct" : "class";
            return kind + " " + type.FullName;
        }

        private static string ApprovedPath()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;

            return Path.Combine(
                projectRoot,
                "Packages",
                "com.lam0uette.uniject",
                "Tests",
                "Unity",
                APPROVED_FILE);
        }
    }
}

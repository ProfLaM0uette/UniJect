using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ForbiddenApiTests
    {
        private static readonly string[] FORBIDDEN_TOKENS =
        {
            "Expression.Compile",
            "System.Linq.Expressions",
            "Reflection.Emit",
            "DynamicMethod",
            "ILGenerator"
        };

        [Test]
        public void RuntimeSources_NeverUseExpressionTreesOrIlEmit()
        {
            List<string> offenders = new List<string>();
            string[] files = Directory.GetFiles(RuntimeRoot(), "*.cs", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                string content = File.ReadAllText(files[i]);

                for (int j = 0; j < FORBIDDEN_TOKENS.Length; j++)
                {
                    if (content.Contains(FORBIDDEN_TOKENS[j]))
                        offenders.Add(Path.GetFileName(files[i]) + " uses " + FORBIDDEN_TOKENS[j]);
                }
            }

            Assert.That(offenders, Is.Empty, string.Join("\n", offenders));
        }

        [Test]
        public void CoreSources_NeverReferenceTheUnityEngine()
        {
            List<string> offenders = new List<string>();
            string[] files = Directory.GetFiles(RuntimeRoot(), "*.cs", SearchOption.AllDirectories);
            string unityFolder = Path.Combine(RuntimeRoot(), "Unity") + Path.DirectorySeparatorChar;

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].StartsWith(unityFolder, System.StringComparison.OrdinalIgnoreCase))
                    continue;

                if (File.ReadAllText(files[i]).Contains("using UnityEngine"))
                    offenders.Add(Path.GetFileName(files[i]));
            }

            Assert.That(offenders, Is.Empty, string.Join("\n", offenders));
        }


        [Test]
        public void RuntimeSources_NeverUseTheVarKeywordOrCollectionExpressions()
        {
            List<string> offenders = new List<string>();
            string[] files = Directory.GetFiles(RuntimeRoot(), "*.cs", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                string[] lines = File.ReadAllLines(files[i]);

                for (int j = 0; j < lines.Length; j++)
                {
                    string trimmed = lines[j].Trim();

                    if (trimmed.StartsWith("var ", System.StringComparison.Ordinal))
                        offenders.Add(Path.GetFileName(files[i]) + ":" + (j + 1) + " uses var");
                }
            }

            Assert.That(offenders, Is.Empty, string.Join("\n", offenders));
        }


        private static string RuntimeRoot()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string runtime = Path.Combine(
                projectRoot,
                "Packages",
                "com.lam0uette.uniject",
                "Runtime");

            Assert.That(Directory.Exists(runtime), Is.True, "Runtime folder not found at " + runtime);

            return runtime;
        }
    }
}

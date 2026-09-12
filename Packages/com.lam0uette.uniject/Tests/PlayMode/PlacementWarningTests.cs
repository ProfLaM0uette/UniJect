using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class PlacementWarningTests
    {
        private readonly List<GameObject> _spawned = new List<GameObject>();
        private readonly List<DIContainer> _containers = new List<DIContainer>();

        [TearDown]
        public void CleanUp()
        {
            for (int i = _containers.Count - 1; i >= 0; i--)
                _containers[i].Dispose();

            _containers.Clear();

            for (int i = _spawned.Count - 1; i >= 0; i--)
            {
                if (_spawned[i] != null)
                    UnityEngine.Object.DestroyImmediate(_spawned[i]);
            }

            _spawned.Clear();
        }

        [Test]
        public void Validate_DontDestroyOnLoadWithAnExplicitParent_ReportsUJ008AsAWarning()
        {
            GameObject parent = NewGameObject("Parent");

            DIContainer container = Build(builder =>
                builder.Bind<MonoProbe>().Parent(parent.transform).DontDestroyOnLoad().AsSingleton());

            ValidationReport report = container.Validate();

            Assert.That(report.ErrorCount, Is.EqualTo(0));
            Assert.That(HasCode(report, IssueCode.UJ008), Is.True);
        }

        [Test]
        public void Validate_DontDestroyOnLoadWithNoParent_DoesNotReportUJ008()
        {
            DIContainer container = Build(builder =>
                builder.Bind<MonoProbe>().DontDestroyOnLoad().AsSingleton());

            Assert.That(HasCode(container.Validate(), IssueCode.UJ008), Is.False);
        }

        [Test]
        public void Validate_DontDestroyOnLoadOnASceneScopedContainer_ReportsUJ009()
        {
            ContainerBuilder rootBuilder = new ContainerBuilder();
            DIContainer root = rootBuilder.Build(Options());
            _containers.Add(root);

            IContainerBuilder childBuilder = root.CreateChildBuilder();
            childBuilder.Bind<MonoProbe>().DontDestroyOnLoad().AsSingleton();

            DIContainer child = childBuilder.Build(Options());
            _containers.Add(child);

            Assert.That(HasCode(child.Validate(), IssueCode.UJ009), Is.True);
        }

        [Test]
        public void Validate_APlainPlacementWithNoDontDestroyOnLoad_IsClean()
        {
            GameObject parent = NewGameObject("Parent");

            DIContainer container = Build(builder =>
                builder.Bind<MonoProbe>().Name("x").Parent(parent.transform).AsSingleton());

            Assert.That(container.Validate().Issues, Is.Empty);
        }


        private static bool HasCode(ValidationReport report, string code)
        {
            for (int i = 0; i < report.Issues.Count; i++)
            {
                if (report.Issues[i].Code == code)
                    return true;
            }

            return false;
        }

        private static ContainerOptions Options()
        {
            ContainerOptions options = UnityContainerOptions.Create();
            options.ValidateOnBuild = false;

            return options;
        }

        private DIContainer Build(Action<IContainerBuilder> declare)
        {
            ContainerBuilder builder = new ContainerBuilder();
            declare(builder);

            DIContainer container = builder.Build(Options());
            _containers.Add(container);

            return container;
        }

        private GameObject NewGameObject(string name)
        {
            GameObject created = new GameObject(name);
            _spawned.Add(created);

            return created;
        }
    }
}

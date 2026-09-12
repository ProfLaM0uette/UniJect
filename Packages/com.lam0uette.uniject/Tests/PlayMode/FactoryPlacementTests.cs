using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class FactoryPlacementTests
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
        public void BindPlaceholderFactory_OnAComponentProduct_CreatesItThroughTheGameObjectPath()
        {
            DIContainer container = Build(builder =>
                builder.BindPlaceholderFactory<MonoProbeFactory, MonoProbe>().Name("spawned"));

            MonoProbe made = container.Resolve<MonoProbeFactory>().Create();
            _spawned.Add(made.gameObject);

            Assert.That(made.gameObject.name, Is.EqualTo("spawned"));
            Assert.That(made.transform.localScale, Is.EqualTo(Vector3.one));
        }

        [Test]
        public void BindPlaceholderFactory_WithParentThenRoot_EndsAtTheSceneRoot()
        {
            GameObject parent = NewGameObject("Parent");

            DIContainer container = Build(builder =>
                builder.BindPlaceholderFactory<MonoProbeFactory, MonoProbe>().Parent(parent.transform).Root());

            MonoProbe made = container.Resolve<MonoProbeFactory>().Create();
            _spawned.Add(made.gameObject);

            Assert.That(made.transform.parent, Is.Null);
        }

        [Test]
        public void BindPlaceholderFactory_CreatedTwice_ProducesTwoDistinctObjects()
        {
            DIContainer container = Build(builder =>
                builder.BindPlaceholderFactory<MonoProbeFactory, MonoProbe>());

            MonoProbeFactory factory = container.Resolve<MonoProbeFactory>();

            MonoProbe first = factory.Create();
            MonoProbe second = factory.Create();

            _spawned.Add(first.gameObject);
            _spawned.Add(second.gameObject);

            Assert.That(first, Is.Not.SameAs(second));
        }


        private DIContainer Build(Action<IContainerBuilder> declare)
        {
            ContainerBuilder builder = new ContainerBuilder();
            declare(builder);

            ContainerOptions options = UnityContainerOptions.Create();
            options.ValidateOnBuild = false;

            DIContainer container = builder.Build(options);
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

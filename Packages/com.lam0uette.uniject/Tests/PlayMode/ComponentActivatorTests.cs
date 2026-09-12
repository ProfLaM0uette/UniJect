using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ComponentActivatorTests
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
        public void FromNewPrefab_ASpawnedComponent_ReadsItsDependencyInsideAwake()
        {
            GameObject template = NewGameObject("Template");
            template.AddComponent<MonoThing>();

            DIContainer container = Build(builder =>
            {
                builder.Bind<IGreeter, UnityGreeter>().AsSingleton();
                builder.Bind<MonoThing>().FromNewPrefab(template).AsSingleton();
            });

            MonoThing spawned = container.Resolve<MonoThing>();
            _spawned.Add(spawned.gameObject);

            Assert.That(spawned.AwakeCount, Is.EqualTo(1));
            Assert.That(spawned.SawGreeterInAwake, Is.True);
        }

        [Test]
        public void FromNewGameObject_ACreatedComponent_ReadsItsDependencyInsideAwake()
        {
            DIContainer container = Build(builder =>
            {
                builder.Bind<IGreeter, UnityGreeter>().AsSingleton();
                builder.Bind<MonoThing>().FromNewGameObject().AsSingleton();
            });

            MonoThing created = container.Resolve<MonoThing>();

            Assert.That(created.AwakeCount, Is.EqualTo(1));
            Assert.That(created.SawGreeterInAwake, Is.True);
        }


        [Test]
        public void FromNewComponentOnGameObject_AlwaysAddsTheComponent()
        {
            GameObject host = NewGameObject("Host");

            DIContainer container = Build(builder =>
            {
                builder.Bind<IGreeter, UnityGreeter>().AsSingleton();
                builder.Bind<MonoThing>().FromNewComponentOnGameObject(host).AsSingleton();
            });

            MonoThing added = container.Resolve<MonoThing>();

            Assert.That(added.gameObject, Is.SameAs(host));
            Assert.That(host.GetComponents<MonoThing>().Length, Is.EqualTo(1));
        }

        [Test]
        public void FromComponentOnGameObject_WhenTheComponentIsAbsent_ThrowsInsteadOfCreatingIt()
        {
            GameObject host = NewGameObject("Host");

            DIContainer container = Build(builder =>
                builder.Bind<MonoProbe>().FromComponentOnGameObject(host).AsSingleton());

            Assert.Throws<BindingNotFoundException>(() => container.Resolve<MonoProbe>());
            Assert.That(host.GetComponent<MonoProbe>(), Is.Null);
        }

        [Test]
        public void FromComponentOnGameObject_WhenTheComponentIsPresent_FindsItWithoutAddingAnother()
        {
            GameObject host = NewGameObject("Host");
            MonoProbe existing = host.AddComponent<MonoProbe>();

            DIContainer container = Build(builder =>
                builder.Bind<MonoProbe>().FromComponentOnGameObject(host).AsSingleton());

            Assert.That(container.Resolve<MonoProbe>(), Is.SameAs(existing));
            Assert.That(host.GetComponents<MonoProbe>().Length, Is.EqualTo(1));
        }

        [Test]
        public void FromComponentInHierarchy_WhenNothingMatches_ThrowsInsteadOfCreatingSilently()
        {
            DIContainer container = Build(builder =>
                builder.Bind<MonoProbe>().FromComponentInHierarchy().AsSingleton());

            Assert.Throws<BindingNotFoundException>(() => container.Resolve<MonoProbe>());
        }


        [UnityTest]
        public IEnumerator Dispose_AContainerThatCreatedTheGameObject_DestroysIt()
        {
            DIContainer container = Build(builder => builder.Bind<MonoProbe>().FromNewGameObject().AsSingleton());
            MonoProbe created = container.Resolve<MonoProbe>();
            GameObject host = created.gameObject;

            container.Dispose();
            _containers.Remove(container);

            yield return null;

            Assert.That(host == null, Is.True);
        }

        [Test]
        public void Dispose_AContainerThatOnlyFoundTheComponent_DestroysNothing()
        {
            GameObject host = NewGameObject("Host");
            host.AddComponent<MonoProbe>();

            DIContainer container = Build(builder =>
                builder.Bind<MonoProbe>().FromComponentOnGameObject(host).AsSingleton());

            container.Resolve<MonoProbe>();
            container.Dispose();
            _containers.Remove(container);

            Assert.That(host == null, Is.False);
            Assert.That(host.GetComponent<MonoProbe>() == null, Is.False);
        }


        [Test]
        public void PlacementVerbs_AreConstrainedToComponent_SoTheyCannotCompileOnAPlainClass()
        {
            MethodInfo name = typeof(MonoPlacementBinderExtensions).GetMethod(nameof(MonoPlacementBinderExtensions.Name));
            Type concrete = name.GetGenericArguments()[1];

            Type[] constraints = concrete.GetGenericParameterConstraints();
            bool requiresComponent = false;

            for (int i = 0; i < constraints.Length; i++)
                requiresComponent |= constraints[i] == typeof(Component);

            Assert.That(requiresComponent, Is.True);
            Assert.That(typeof(Component).IsAssignableFrom(typeof(PlainThing)), Is.False);
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

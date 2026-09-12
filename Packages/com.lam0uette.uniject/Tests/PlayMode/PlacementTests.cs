using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class PlacementTests
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
                    Object.DestroyImmediate(_spawned[i]);
            }

            _spawned.Clear();
        }

        [Test]
        public void FromNewGameObject_WithNoPoseVerb_LeavesTheTransformAlone()
        {
            DIContainer container = Build(builder => builder.Bind<MonoProbe>().FromNewGameObject().AsSingleton());
            MonoProbe probe = container.Resolve<MonoProbe>();

            Assert.That(probe.transform.localScale, Is.EqualTo(Vector3.one));
            Assert.That(probe.transform.localRotation, Is.EqualTo(Quaternion.identity));
            Assert.That(probe.transform.localPosition, Is.EqualTo(Vector3.zero));
        }

        [Test]
        public void FromNewPrefab_OnATemplateWithAnAuthoredTransform_KeepsIt()
        {
            GameObject template = NewGameObject("Template");
            template.transform.localScale = new Vector3(3f, 3f, 3f);
            template.transform.localPosition = new Vector3(5f, 0f, 0f);
            template.AddComponent<MonoProbe>();

            DIContainer container = Build(builder =>
                builder.Bind<MonoProbe>().FromNewPrefab(template).AsSingleton());

            MonoProbe spawned = container.Resolve<MonoProbe>();
            _spawned.Add(spawned.gameObject);

            Assert.That(spawned.transform.localScale, Is.EqualTo(new Vector3(3f, 3f, 3f)));
            Assert.That(spawned.transform.localPosition, Is.EqualTo(new Vector3(5f, 0f, 0f)));
        }

        [Test]
        public void Scale_WhenSet_IsTheOnlyComponentWritten()
        {
            DIContainer container = Build(builder =>
                builder.Bind<MonoProbe>().FromNewGameObject().Scale(new Vector3(2f, 2f, 2f)).AsSingleton());

            MonoProbe probe = container.Resolve<MonoProbe>();

            Assert.That(probe.transform.localScale, Is.EqualTo(new Vector3(2f, 2f, 2f)));
            Assert.That(probe.transform.localRotation, Is.EqualTo(Quaternion.identity));
        }


        [Test]
        public void Name_WithNoFromVerbAtAll_StillCreatesAndPlacesTheGameObject()
        {
            GameObject parent = NewGameObject("Parent");

            DIContainer container = Build(builder =>
                builder.Bind<MonoProbe>().Name("placed").Parent(parent.transform).AsSingleton());

            MonoProbe probe = container.Resolve<MonoProbe>();

            Assert.That(probe.gameObject.name, Is.EqualTo("placed"));
            Assert.That(probe.transform.parent, Is.SameAs(parent.transform));
        }

        [Test]
        public void Name_WhenNotSet_FallsBackToTheConcreteTypeName()
        {
            DIContainer container = Build(builder => builder.Bind<MonoProbe>().FromNewGameObject().AsSingleton());

            Assert.That(container.Resolve<MonoProbe>().gameObject.name, Is.EqualTo(nameof(MonoProbe)));
        }

        [Test]
        public void ParentThenRoot_LastWriteWins_AndTheObjectEndsAtTheSceneRoot()
        {
            GameObject parent = NewGameObject("Parent");

            DIContainer container = Build(builder =>
                builder.Bind<MonoProbe>().Parent(parent.transform).Root().AsSingleton());

            Assert.That(container.Resolve<MonoProbe>().transform.parent, Is.Null);
        }


        private DIContainer Build(System.Action<IContainerBuilder> declare)
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

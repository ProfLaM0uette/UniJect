using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class SceneContextTests
    {
        private readonly List<GameObject> _spawned = new List<GameObject>();

        [TearDown]
        public void DestroySpawned()
        {
            for (int i = _spawned.Count - 1; i >= 0; i--)
            {
                if (_spawned[i] != null)
                    Object.DestroyImmediate(_spawned[i]);
            }

            _spawned.Clear();
        }

        [Test]
        public void Awake_WithAnInstaller_BuildsAChildOfTheProjectContainer()
        {
            SceneContext context = CreateContext();

            Assert.That(context.Container, Is.Not.Null);
            Assert.That(context.Container.Parent, Is.SameAs(ProjectContext.Container));
            Assert.That(context.Container.Root, Is.SameAs(ProjectContext.Container));
            Assert.That(context.Container.Resolve<IScore>(), Is.InstanceOf<SceneScore>());
        }

        [Test]
        public void Awake_TwoSceneContexts_GetSeparateContainersUnderOneProjectRoot()
        {
            SceneContext first = CreateContext();
            SceneContext second = CreateContext();

            Assert.That(first.Container, Is.Not.SameAs(second.Container));
            Assert.That(first.Container.Resolve<IScore>(), Is.Not.SameAs(second.Container.Resolve<IScore>()));
            Assert.That(first.Container.Root, Is.SameAs(second.Container.Root));
        }

        [Test]
        public void Awake_ASceneContext_RegistersItsContainerForItsScene()
        {
            SceneContext context = CreateContext();

            Assert.That(SceneScopeRegistry.TryGet(context.gameObject.scene, out DIContainer registered), Is.True);
            Assert.That(registered, Is.SameAs(context.Container));
        }


        [Test]
        public void OnDestroy_ASceneContext_DisposesItsContainerAndLeavesTheProjectOneAlone()
        {
            SceneContext context = CreateContext();
            DIContainer scoped = context.Container;

            Object.DestroyImmediate(context.gameObject);
            _spawned.Clear();

            Assert.That(scoped.IsDisposed, Is.True);
            Assert.That(ProjectContext.Container.IsDisposed, Is.False);
        }


        private SceneContext CreateContext()
        {
            GameObject host = new GameObject(nameof(SceneContextTests));
            host.SetActive(false);
            _spawned.Add(host);

            ScoreInstaller installer = host.AddComponent<ScoreInstaller>();
            SceneContext context = host.AddComponent<SceneContext>();

            FieldInfo field = typeof(SceneContext).GetField(
                "_installers",
                BindingFlags.Instance | BindingFlags.NonPublic);

            field.SetValue(context, new MonoInstallerBase[] { installer });
            host.SetActive(true);

            return context;
        }
    }
}

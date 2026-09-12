using NUnit.Framework;
using UnityEngine.SceneManagement;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class UniJectStaticsResetTests
    {
        [Test]
        public void Reset_AfterAProjectContainerExists_DropsItSoTheNextPlaySessionRebuildsIt()
        {
            DIContainer before = ProjectContext.Container;
            Assert.That(ProjectContext.Exists, Is.True);

            UniJectStatics.Reset();

            Assert.That(ProjectContext.Exists, Is.False);

            before.Dispose();

            Assert.That(ProjectContext.Container, Is.Not.SameAs(before));
        }

        [Test]
        public void Reset_AfterASceneIsRegistered_EmptiesTheRegistry()
        {
            Scene scene = SceneManager.GetActiveScene();

            ContainerBuilder builder = new ContainerBuilder();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                SceneScopeRegistry.Register(scene, container);
                Assert.That(SceneScopeRegistry.TryGet(scene, out DIContainer _), Is.True);

                UniJectStatics.Reset();

                Assert.That(SceneScopeRegistry.TryGet(scene, out DIContainer _), Is.False);
            }
        }

        [Test]
        public void Reset_RunTwice_IsIdempotent()
        {
            UniJectStatics.Reset();
            UniJectStatics.Reset();

            Assert.That(ProjectContext.Exists, Is.False);
        }
    }
}

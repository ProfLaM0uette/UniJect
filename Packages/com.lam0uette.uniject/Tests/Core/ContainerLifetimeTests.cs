using System.Reflection;
using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ContainerLifetimeTests
    {
        [SetUp]
        public void ResetCounters()
        {
            GameState.CreationCount = 0;
        }

        [Test]
        public void AsSingleton_ResolvedTwice_ReturnsTheSameInstance()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<GameState>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<GameState>(), Is.SameAs(container.Resolve<GameState>()));
                Assert.That(GameState.CreationCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void AsSingleton_AliasedFromAnotherContract_SharesOneInstance()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<MemoryLog>().AsSingleton();
            builder.Bind<ILog, MemoryLog>().FromResolve().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<MemoryLog>(), Is.SameAs(container.Resolve<ILog>()));
            }
        }

        [Test]
        public void FromResolve_PointingAtItsOwnContract_ThrowsAtBuild()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<MemoryLog>().FromResolve().AsSingleton();

            InvalidBindingException exception =
                Assert.Throws<InvalidBindingException>(() => builder.Build(ContainerOptions.Relaxed));

            Assert.That(exception.Code, Is.EqualTo(IssueCode.UJ007));
        }


        [Test]
        public void AsTransient_ResolvedTwice_ReturnsDistinctInstances()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<GameState>().AsTransient();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<GameState>(), Is.Not.SameAs(container.Resolve<GameState>()));
                Assert.That(GameState.CreationCount, Is.EqualTo(2));
            }
        }

        [Test]
        public void AsTransient_CombinedWithNonLazy_ThrowsAtBuild()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<GameState>().AsTransient().NonLazy();

            InvalidBindingException exception =
                Assert.Throws<InvalidBindingException>(() => builder.Build(ContainerOptions.Relaxed));

            Assert.That(exception.Code, Is.EqualTo(IssueCode.UJ005));
        }


        [Test]
        public void NonLazy_AfterBuild_IsCreatedBeforeAnyResolve()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<GameState>().AsSingleton().NonLazy();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(GameState.CreationCount, Is.EqualTo(0));

                container.ResolveNonLazy();
                Assert.That(GameState.CreationCount, Is.EqualTo(1));
            }
        }


        [Test]
        public void ContainerBuilder_PublicSurface_ExposesNoResolveDuringInstall()
        {
            MethodInfo[] methods = typeof(IContainerBuilder).GetMethods();

            foreach (MethodInfo method in methods)
            {
                Assert.That(method.Name, Is.Not.EqualTo("Resolve"));
                Assert.That(method.Name, Is.Not.EqualTo("CreateInstance"));
                Assert.That(method.Name, Is.Not.EqualTo("Inject"));
            }
        }
    }
}

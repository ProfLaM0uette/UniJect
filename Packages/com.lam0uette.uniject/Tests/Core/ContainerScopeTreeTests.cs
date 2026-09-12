using System.Collections.Generic;
using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ContainerScopeTreeTests
    {
        [SetUp]
        public void ResetCounters()
        {
            ScopedService.CreationCount = 0;
            GameState.CreationCount = 0;
        }

        [Test]
        public void Resolve_AParentSingletonFromTwoChildren_ReturnsTheSameInstance()
        {
            ContainerBuilder root = new ContainerBuilder();
            root.Bind<GameState>().AsSingleton();

            using (DIContainer parent = root.Build(ContainerOptions.Relaxed))
            {
                DIContainer first = parent.CreateChildBuilder().Build(ContainerOptions.Relaxed);
                DIContainer second = parent.CreateChildBuilder().Build(ContainerOptions.Relaxed);

                Assert.That(first.Resolve<GameState>(), Is.SameAs(second.Resolve<GameState>()));
                Assert.That(GameState.CreationCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void Resolve_AChildBindingThatShadowsTheParent_PrefersTheNearestScope()
        {
            ContainerBuilder root = new ContainerBuilder();
            root.Bind<IWeapon, Sword>().AsSingleton();

            using (DIContainer parent = root.Build(ContainerOptions.Relaxed))
            {
                IContainerBuilder childBuilder = parent.CreateChildBuilder();
                childBuilder.Bind<IWeapon, Bow>().AsSingleton();

                DIContainer child = childBuilder.Build(ContainerOptions.Relaxed);

                Assert.That(child.Resolve<IWeapon>(), Is.InstanceOf<Bow>());
                Assert.That(parent.Resolve<IWeapon>(), Is.InstanceOf<Sword>());
            }
        }

        [Test]
        public void Resolve_ASingletonDeclaredInAChild_IsStoredInThatChildNotInTheRoot()
        {
            ContainerBuilder root = new ContainerBuilder();
            root.Bind<IWeapon, Sword>().AsSingleton();

            using (DIContainer parent = root.Build(ContainerOptions.Relaxed))
            {
                IContainerBuilder childBuilder = parent.CreateChildBuilder();
                childBuilder.Bind<GameState>().AsSingleton();

                DIContainer child = childBuilder.Build(ContainerOptions.Relaxed);

                Assert.That(child.Resolve<GameState>(), Is.SameAs(child.Resolve<GameState>()));
                Assert.That(parent.Resolve<IWeapon>(), Is.InstanceOf<Sword>());
                Assert.That(GameState.CreationCount, Is.EqualTo(1));
            }
        }


        [Test]
        public void AsScoped_ResolvedFromTwoChildren_GivesEachChildItsOwnInstance()
        {
            ContainerBuilder root = new ContainerBuilder();
            root.Bind<ScopedService>().AsScoped();

            using (DIContainer parent = root.Build(ContainerOptions.Relaxed))
            {
                DIContainer first = parent.CreateChildBuilder().Build(ContainerOptions.Relaxed);
                DIContainer second = parent.CreateChildBuilder().Build(ContainerOptions.Relaxed);

                Assert.That(first.Resolve<ScopedService>(), Is.Not.SameAs(second.Resolve<ScopedService>()));
                Assert.That(first.Resolve<ScopedService>(), Is.SameAs(first.Resolve<ScopedService>()));
                Assert.That(ScopedService.CreationCount, Is.EqualTo(2));
            }
        }


        [Test]
        public void Dispose_AChild_ReleasesOnlyItsOwnInstances()
        {
            DisposalRecorder recorder = new DisposalRecorder();

            ContainerBuilder root = new ContainerBuilder();
            root.BindInstance(recorder);
            root.Bind<FirstDisposable>().AsSingleton();

            using (DIContainer parent = root.Build(ContainerOptions.Relaxed))
            {
                IContainerBuilder childBuilder = parent.CreateChildBuilder();
                childBuilder.Bind<SecondDisposable>().AsSingleton();

                DIContainer child = childBuilder.Build(ContainerOptions.Relaxed);
                child.Resolve<SecondDisposable>();

                child.Dispose();

                Assert.That(recorder.Order, Is.EqualTo(new List<string> { nameof(SecondDisposable) }));
            }

            Assert.That(recorder.Order, Is.EqualTo(new List<string> { nameof(SecondDisposable), nameof(FirstDisposable) }));
        }

        [Test]
        public void Dispose_AParent_DisposesItsChildrenFirst()
        {
            DisposalRecorder recorder = new DisposalRecorder();

            ContainerBuilder root = new ContainerBuilder();
            root.BindInstance(recorder);
            root.Bind<FirstDisposable>().AsSingleton();

            DIContainer parent = root.Build(ContainerOptions.Relaxed);
            parent.Resolve<FirstDisposable>();

            IContainerBuilder childBuilder = parent.CreateChildBuilder();
            childBuilder.Bind<SecondDisposable>().AsSingleton();

            DIContainer child = childBuilder.Build(ContainerOptions.Relaxed);
            child.Resolve<SecondDisposable>();

            parent.Dispose();

            Assert.That(
                recorder.Order,
                Is.EqualTo(new List<string> { nameof(SecondDisposable), nameof(FirstDisposable) }));
            Assert.That(child.IsDisposed, Is.True);
        }
    }
}

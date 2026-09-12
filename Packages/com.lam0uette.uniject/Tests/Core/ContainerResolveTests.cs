using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ContainerResolveTests
    {
        [Test]
        public void Resolve_BoundContract_ReturnsTheConcreteType()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ILog, MemoryLog>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<ILog>(), Is.InstanceOf<MemoryLog>());
            }
        }

        [Test]
        public void Resolve_UnboundContract_ThrowsInsteadOfReturningNull()
        {
            ContainerBuilder builder = new ContainerBuilder();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                BindingNotFoundException exception =
                    Assert.Throws<BindingNotFoundException>(() => container.Resolve<IInventory>());

                Assert.That(exception.ContractType, Is.EqualTo(typeof(IInventory)));
                Assert.That(exception.Message, Does.StartWith("UniJect: "));
            }
        }

        [Test]
        public void Resolve_UnregisteredNestedDependency_ExposesTheFullResolutionPath()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<GameController>().AsSingleton();
            builder.Bind<PlayerService>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                BindingNotFoundException exception =
                    Assert.Throws<BindingNotFoundException>(() => container.Resolve<GameController>());

                Assert.That(exception.ContractType, Is.EqualTo(typeof(IInventory)));
                Assert.That(
                    exception.Path.ToTypeList(),
                    Is.EqualTo(new List<Type> { typeof(GameController), typeof(PlayerService) }));
                Assert.That(exception.Message, Does.Not.Contain("TargetInvocationException"));
            }
        }

        [Test]
        public void Resolve_ConstructorCycle_ThrowsCircularDependencyInsteadOfOverflowingTheStack()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<CycleA>().AsSingleton();
            builder.Bind<CycleB>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                CircularDependencyException exception =
                    Assert.Throws<CircularDependencyException>(() => container.Resolve<CycleA>());

                Assert.That(exception.Path.Describe(), Does.Contain(nameof(CycleB)));
            }
        }

        [Test]
        public void Resolve_FromMethodSource_ReturnsTheProductNotTheDelegate()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IClock>().FromMethod(resolver => new SystemClock()).AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<IClock>(), Is.InstanceOf<SystemClock>());
            }
        }

        [Test]
        public void Resolve_FromResolveSource_ReturnsTheInstanceNotAnotherDelegate()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<MemoryLog>().AsSingleton();
            builder.Bind<ILog, MemoryLog>().FromResolve().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<ILog>(), Is.SameAs(container.Resolve<MemoryLog>()));
            }
        }


        [Test]
        public void TryResolve_UnboundContract_ReturnsFalseWithoutThrowing()
        {
            ContainerBuilder builder = new ContainerBuilder();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.TryResolve(typeof(IInventory), out object _), Is.False);
            }
        }


        [Test]
        public void GetService_UnboundContract_ReturnsNullPerTheServiceProviderContract()
        {
            ContainerBuilder builder = new ContainerBuilder();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(((IServiceProvider)container).GetService(typeof(IInventory)), Is.Null);
            }
        }


        [Test]
        public void Resolve_AfterDispose_ThrowsObjectDisposed()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ILog, MemoryLog>().AsSingleton();

            DIContainer container = builder.Build(ContainerOptions.Relaxed);
            container.Dispose();

            Assert.Throws<ObjectDisposedException>(() => container.Resolve<ILog>());
        }
    }
}

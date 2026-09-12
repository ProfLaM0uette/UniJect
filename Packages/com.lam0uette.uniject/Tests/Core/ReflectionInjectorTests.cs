using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ReflectionInjectorTests
    {
        [Test]
        public void Inject_PrivateField_IsFilledFromTheContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ILog, MemoryLog>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                FieldConsumer consumer = new FieldConsumer();
                container.Inject(consumer);

                Assert.That(consumer.Log, Is.InstanceOf<MemoryLog>());
            }
        }

        [Test]
        public void Inject_SameTargetTwice_InjectsExactlyOnce()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ILog, MemoryLog>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                FieldConsumer consumer = new FieldConsumer();
                container.Inject(consumer);

                ILog first = consumer.Log;
                container.Inject(consumer);

                Assert.That(consumer.Log, Is.SameAs(first));
            }
        }

        [Test]
        public void Inject_MutuallyReferencingSingletons_CompletesInsteadOfOverflowingTheStack()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<MutualA>().AsSingleton();
            builder.Bind<MutualB>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                MutualA first = container.Resolve<MutualA>();

                Assert.That(first.Other, Is.Not.Null);
                Assert.That(first.Other.Other, Is.SameAs(first));
            }
        }

        [Test]
        public void Inject_OptionalFieldWithNoBinding_LeavesTheFieldAlone()
        {
            ContainerBuilder builder = new ContainerBuilder();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                OptionalConsumer consumer = new OptionalConsumer();
                container.Inject(consumer);

                Assert.That(consumer.Inventory, Is.Null);
            }
        }


        [Test]
        public void CreateInstance_UnboundConcreteType_ResolvesItsConstructorDependencies()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ILog, MemoryLog>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                FieldConsumer consumer = container.CreateInstance<FieldConsumer>();
                Assert.That(consumer.Log, Is.InstanceOf<MemoryLog>());
            }
        }
    }
}

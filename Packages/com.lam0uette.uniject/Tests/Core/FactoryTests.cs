using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class FactoryTests
    {
        [SetUp]
        public void ResetCounters()
        {
            Coin.CreationCount = 0;
        }

        [Test]
        public void BindFactory_WithNoParameters_ProducesANewInstanceOnEveryCreate()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.BindFactory<Coin>();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                IFactory<Coin> factory = container.Resolve<IFactory<Coin>>();

                Assert.That(factory.Create(), Is.Not.SameAs(factory.Create()));
                Assert.That(Coin.CreationCount, Is.EqualTo(2));
            }
        }

        [Test]
        public void BindFactory_TheFactoryItself_IsASingleton()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.BindFactory<Coin>();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<IFactory<Coin>>(), Is.SameAs(container.Resolve<IFactory<Coin>>()));
            }
        }

        [Test]
        public void BindFactory_WithFromMethod_ActuallyCallsTheDelegate()
        {
            int calls = 0;

            ContainerBuilder builder = new ContainerBuilder();
            builder.BindFactory<Coin>().FromMethod(resolver =>
            {
                calls++;
                return new Coin();
            });

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                IFactory<Coin> factory = container.Resolve<IFactory<Coin>>();

                Assert.That(factory.Create(), Is.Not.Null);
                Assert.That(factory.Create(), Is.Not.Null);
                Assert.That(calls, Is.EqualTo(2));
            }
        }

        [Test]
        public void BindFactory_WithParameterlessFromMethod_ActuallyCallsTheDelegate()
        {
            int calls = 0;

            ContainerBuilder builder = new ContainerBuilder();
            builder.BindFactory<Coin>().FromMethod(() =>
            {
                calls++;
                return new Coin();
            });

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                container.Resolve<IFactory<Coin>>().Create();
                Assert.That(calls, Is.EqualTo(1));
            }
        }


        [Test]
        public void BindFactory_WithOneParameter_PassesItToTheProductConstructor()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ILog, MemoryLog>().AsSingleton();
            builder.BindFactory<Bullet, int>();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Bullet bullet = container.Resolve<IFactory<Bullet, int>>().Create(7);

                Assert.That(bullet.Damage, Is.EqualTo(7));
                Assert.That(bullet.Log, Is.InstanceOf<MemoryLog>());
            }
        }

        [Test]
        public void BindFactory_WhoseParameterMatchesNoProductConstructor_ThrowsAtBuild()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.BindFactory<Mismatched, int>();

            InvalidBindingException exception =
                Assert.Throws<InvalidBindingException>(() => builder.Build(ContainerOptions.Relaxed));

            Assert.That(exception.Code, Is.EqualTo(IssueCode.UJ013));
        }


        [Test]
        public void Inject_ATypeImplementingIInjectable_CallsTheInterfaceMethod()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ILog, MemoryLog>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                InjectableConsumer consumer = new InjectableConsumer();
                container.Inject(consumer);

                Assert.That(consumer.Log, Is.InstanceOf<MemoryLog>());
            }
        }
    }
}

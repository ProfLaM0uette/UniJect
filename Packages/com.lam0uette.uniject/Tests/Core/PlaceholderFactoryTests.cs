using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class PlaceholderFactoryTests
    {
        [Test]
        public void BindPlaceholderFactory_ResolvingTheFactory_ProducesTheProductWithItsArgument()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ILog, MemoryLog>().AsSingleton();
            builder.BindPlaceholderFactory<BulletFactory, Bullet, int>();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Bullet bullet = container.Resolve<BulletFactory>().Create(4);

                Assert.That(bullet.Damage, Is.EqualTo(4));
                Assert.That(bullet.Log, Is.InstanceOf<MemoryLog>());
            }
        }

        [Test]
        public void BindPlaceholderFactory_AnOverriddenCreate_IsTheOneThatRuns()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ILog, MemoryLog>().AsSingleton();
            builder.BindPlaceholderFactory<DoubleDamageBulletFactory, Bullet, int>();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<DoubleDamageBulletFactory>().Create(4).Damage, Is.EqualTo(8));
            }
        }

        [Test]
        public void BindPlaceholderFactoryTo_ResolvesThroughTheDeclaredInterface()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ILog, MemoryLog>().AsSingleton();
            builder.BindPlaceholderFactoryTo<IFactory<Bullet, int>, BulletFactory, Bullet, int>();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<IFactory<Bullet, int>>().Create(3).Damage, Is.EqualTo(3));
            }
        }

        [Test]
        public void BindPlaceholderFactory_WithAParamsArgument_PassesTheWholeBag()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.BindPlaceholderFactory<LabelledFactory, Labelled, IParams<string, int>>();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Labelled made = container.Resolve<LabelledFactory>().Create(new Params<string, int>("hp", 12));

                Assert.That(made.Params.P1, Is.EqualTo("hp"));
                Assert.That(made.Params.P2, Is.EqualTo(12));
            }
        }


        [Test]
        public void Params_OfThreeArguments_IsNotAssignableToParamsOfTwo()
        {
            Assert.That(
                typeof(IParams<string, int>).IsAssignableFrom(typeof(Params<string, int, float>)),
                Is.False);
        }
    }
}

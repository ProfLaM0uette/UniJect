using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ContainerCircularDependencyTests
    {
        [Test]
        public void Build_WithValidateOnBuildAndAConstructorCycle_ThrowsWithThePath()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<CycleA>().AsSingleton();
            builder.Bind<CycleB>().AsSingleton();

            ContainerValidationException exception =
                Assert.Throws<ContainerValidationException>(() => builder.Build(ContainerOptions.Strict));

            Assert.That(exception.Report.ErrorCount, Is.GreaterThan(0));
            Assert.That(exception.Report.Issues[0].Message, Does.Contain("circular dependency"));
            Assert.That(exception.Report.Issues[0].Message, Does.Contain(nameof(CycleB)));
        }

        [Test]
        public void Build_AMemberCycleBetweenSingletons_IsAcceptedBecauseTheCacheBreaksIt()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<MutualA>().AsSingleton();
            builder.Bind<MutualB>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Strict))
            {
                MutualA first = container.Resolve<MutualA>();

                Assert.That(first.Other.Other, Is.SameAs(first));
            }
        }


        [Test]
        public void Resolve_AConstructorCycleWithValidationOff_StillThrowsInsteadOfOverflowing()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<CycleA>().AsSingleton();
            builder.Bind<CycleB>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                CircularDependencyException exception =
                    Assert.Throws<CircularDependencyException>(() => container.Resolve<CycleA>());

                Assert.That(exception.Path.Describe(), Does.Contain(nameof(CycleA)));
                Assert.That(exception.Path.Describe(), Does.Contain(nameof(CycleB)));
            }
        }
    }
}

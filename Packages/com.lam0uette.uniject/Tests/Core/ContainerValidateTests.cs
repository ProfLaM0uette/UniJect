using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ContainerValidateTests
    {
        [SetUp]
        public void ResetCounters()
        {
            GameState.CreationCount = 0;
        }

        [Test]
        public void Validate_AHealthyGraph_ReturnsAnEmptyReport()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().AsSingleton();
            builder.Bind<Warrior>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                ValidationReport report = container.Validate();

                Assert.That(report.Issues, Is.Empty);
                Assert.That(report.ErrorCount, Is.EqualTo(0));
            }
        }

        [Test]
        public void Validate_AHealthyGraph_InstantiatesNothing()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<GameState>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                container.Validate();
                Assert.That(GameState.CreationCount, Is.EqualTo(0));
            }
        }

        [Test]
        public void Validate_AMissingDependency_ReportsAnErrorInsteadOfThrowing()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<Warrior>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                ValidationReport report = container.Validate();

                Assert.That(report.ErrorCount, Is.EqualTo(1));
                Assert.That(report.Issues[0].Code, Is.EqualTo(IssueCode.UJ001));
                Assert.That(report.Issues[0].Message, Does.Contain(nameof(IWeapon)));
            }
        }

        [Test]
        public void Validate_SeveralBrokenGraphs_CollectsThemAllInsteadOfStoppingAtTheFirst()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<Warrior>().AsSingleton();
            builder.Bind<Archer>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Validate().ErrorCount, Is.EqualTo(2));
            }
        }


        [Test]
        public void Build_WithValidateOnBuildAndAMissingDependency_ThrowsBeforeAnythingResolves()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<Warrior>().AsSingleton();

            ContainerValidationException exception =
                Assert.Throws<ContainerValidationException>(() => builder.Build(ContainerOptions.Strict));

            Assert.That(exception.Report.ErrorCount, Is.EqualTo(1));
        }

        [Test]
        public void Build_ASingletonDependingOnAScopedService_IsACaptiveDependencyError()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ScopedService>().AsScoped();
            builder.Bind<SingletonWithScopedDependency>().AsSingleton();

            ContainerValidationException exception =
                Assert.Throws<ContainerValidationException>(() => builder.Build(ContainerOptions.Strict));

            Assert.That(exception.Report.Issues[0].Code, Is.EqualTo(IssueCode.UJ012));
            Assert.That(exception.Report.Issues[0].Message, Does.Contain(nameof(ScopedService)));
        }

        [Test]
        public void Build_ASingletonDependingOnAScopedServiceWithScopeCheckingOff_IsAccepted()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ScopedService>().AsScoped();
            builder.Bind<SingletonWithScopedDependency>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<SingletonWithScopedDependency>().Service, Is.Not.Null);
            }
        }


        [Test]
        public void Build_WithValidateOnBuildAndAHealthyGraph_Succeeds()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().AsSingleton();
            builder.Bind<Warrior>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Strict))
            {
                Assert.That(container.Resolve<Warrior>().Weapon, Is.InstanceOf<Sword>());
            }
        }
    }
}

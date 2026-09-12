using System.Collections.Generic;
using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ExceptionMessageTests
    {
        [Test]
        public void BindingNotFound_WhenTheContainerIsDescribed_NamesItAndItsInstallers()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<Warrior>().AsSingleton();

            ContainerOptions options = ContainerOptions.Relaxed;
            options.Description = "SceneContext 'Level01'";
            options.InstallerNames = new List<string> { "GameInstaller", "UiInstaller" };

            using (DIContainer container = builder.Build(options))
            {
                BindingNotFoundException exception =
                    Assert.Throws<BindingNotFoundException>(() => container.Resolve<Warrior>());

                Assert.That(exception.Message, Does.Contain("Container: SceneContext 'Level01'"));
                Assert.That(exception.Message, Does.Contain("Installers: GameInstaller, UiInstaller"));
            }
        }

        [Test]
        public void BindingNotFound_WithNoDescription_OmitsTheContextBlockEntirely()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<Warrior>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                BindingNotFoundException exception =
                    Assert.Throws<BindingNotFoundException>(() => container.Resolve<Warrior>());

                Assert.That(exception.Message, Does.Not.Contain("Container:"));
                Assert.That(exception.Message, Does.Not.Contain("Installers:"));
            }
        }

        [Test]
        public void AmbiguousBinding_WhenTheContainerIsDescribed_NamesIt()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().AsSingleton();
            builder.Bind<IWeapon, Bow>().AsSingleton();

            ContainerOptions options = ContainerOptions.Relaxed;
            options.Description = "ProjectContext";

            using (DIContainer container = builder.Build(options))
            {
                AmbiguousBindingException exception =
                    Assert.Throws<AmbiguousBindingException>(() => container.Resolve<IWeapon>());

                Assert.That(exception.Message, Does.Contain("Container: ProjectContext"));
            }
        }


        [Test]
        public void Validate_TwoDistinctSourcesOnOneBinding_ReportsUJ004AsAWarning()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ILog, MemoryLog>().FromInstance(new MemoryLog()).FromMethod(() => new MemoryLog())
                .AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                ValidationReport report = container.Validate();

                Assert.That(report.ErrorCount, Is.EqualTo(0));
                Assert.That(report.WarningCount, Is.EqualTo(1));
                Assert.That(report.Issues[0].Code, Is.EqualTo(IssueCode.UJ004));
            }
        }

        [Test]
        public void Build_AWarningOnly_NeverBlocksTheBuild()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<ILog, MemoryLog>().FromInstance(new MemoryLog()).FromMethod(() => new MemoryLog())
                .AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Strict))
            {
                Assert.That(container.Resolve<ILog>(), Is.Not.Null);
            }
        }
    }
}

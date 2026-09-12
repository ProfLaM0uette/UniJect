using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ContainerWindowModelTests
    {
        [Test]
        public void FromContainer_ABoundContract_ProducesOneRowCarryingItsOrigin()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWindowService, WindowService>().AsSingleton().NonLazy();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                ContainerWindowModel model = ContainerWindowModel.FromContainer("test", container, null);
                ContainerBindingRow row = Find(model, nameof(IWindowService));

                Assert.That(row.Concrete, Is.EqualTo(nameof(WindowService)));
                Assert.That(row.Lifetime, Is.EqualTo(nameof(Lifetime.Singleton)));
                Assert.That(row.NonLazy, Is.True);
                Assert.That(row.Origin.IsKnown, Is.True);
                Assert.That(row.Origin.ToString(), Does.Contain("ContainerWindowModelTests.cs:"));
            }
        }

        [Test]
        public void FromContainer_ABindingNeverResolved_ReportsItAsNotCreated()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWindowService, WindowService>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(Find(ContainerWindowModel.FromContainer("test", container, null), nameof(IWindowService))
                    .State, Is.EqualTo("not created"));
            }
        }

        [Test]
        public void FromContainer_AConditionalBinding_ShowsTheCondition()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWindowService, WindowService>().WhenInjectedInto<WindowConsumer>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(
                    Find(ContainerWindowModel.FromContainer("test", container, null), nameof(IWindowService))
                        .Condition,
                    Does.Contain("WhenInjectedInto"));
            }
        }

        [Test]
        public void FromContainer_AnIdCarryingBinding_ShowsTheId()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWindowService, WindowService>().WithId("main").AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(
                    Find(ContainerWindowModel.FromContainer("test", container, null), nameof(IWindowService)).Id,
                    Is.EqualTo("main"));
            }
        }


        [Test]
        public void FromContainer_NoContainerAtAll_ProducesAnEmptyModelInsteadOfThrowing()
        {
            ContainerWindowModel model = ContainerWindowModel.FromContainer("nothing", null, null);

            Assert.That(model.Rows, Is.Empty);
            Assert.That(model.Source, Is.EqualTo("nothing"));
        }


        private static ContainerBindingRow Find(ContainerWindowModel model, string contract)
        {
            for (int i = 0; i < model.Rows.Count; i++)
            {
                if (model.Rows[i].Contract == contract)
                    return model.Rows[i];
            }

            Assert.Fail("No row for " + contract);
            return null;
        }
    }
}

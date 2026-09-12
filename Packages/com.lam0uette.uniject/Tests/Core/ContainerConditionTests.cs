using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ContainerConditionTests
    {
        [Test]
        public void Resolve_TwoUnconditionalBindingsOfOneContract_ThrowsShowingBothCandidates()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().AsSingleton();
            builder.Bind<IWeapon, Bow>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                AmbiguousBindingException exception =
                    Assert.Throws<AmbiguousBindingException>(() => container.Resolve<IWeapon>());

                Assert.That(exception.Candidates.Count, Is.EqualTo(2));
                Assert.That(exception.Message, Does.Contain(nameof(Sword)));
                Assert.That(exception.Message, Does.Contain(nameof(Bow)));
                Assert.That(exception.Message, Does.Contain("ContainerConditionTests.cs:"));
            }
        }

        [Test]
        public void Resolve_WhenInjectedIntoTheMatchingConsumer_SelectsTheConditionalBinding()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().WhenInjectedInto<Warrior>().AsSingleton();
            builder.Bind<Warrior>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<Warrior>().Weapon, Is.InstanceOf<Sword>());
            }
        }

        [Test]
        public void Resolve_WhenInjectedIntoAnotherConsumer_ThrowsInsteadOfHandingItOver()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().WhenInjectedInto<Warrior>().AsSingleton();
            builder.Bind<Archer>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                BindingNotFoundException exception =
                    Assert.Throws<BindingNotFoundException>(() => container.Resolve<Archer>());

                Assert.That(exception.ContractType, Is.EqualTo(typeof(IWeapon)));
                Assert.That(exception.Message, Does.Contain("WhenInjectedInto"));
            }
        }

        [Test]
        public void Resolve_ASatisfiedConditionAndAnUnconditionalBinding_PrefersTheCondition()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Bow>().AsSingleton();
            builder.Bind<IWeapon, Sword>().WhenInjectedInto<Warrior>().AsSingleton();
            builder.Bind<Warrior>().AsSingleton();
            builder.Bind<Archer>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<Warrior>().Weapon, Is.InstanceOf<Sword>());
                Assert.That(container.Resolve<Archer>().Weapon, Is.InstanceOf<Bow>());
            }
        }

        [Test]
        public void Resolve_TwoConditionsThatBothMatch_ThrowsInsteadOfPickingAWinner()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().WhenInjectedInto<Warrior>().AsSingleton();
            builder.Bind<IWeapon, Bow>().WhenInjectedInto<Warrior>().AsSingleton();
            builder.Bind<Warrior>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.Throws<AmbiguousBindingException>(() => container.Resolve<Warrior>());
            }
        }

        [Test]
        public void Resolve_WhenNotInjectedInto_ExcludesTheNamedConsumer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().WhenNotInjectedInto<Archer>().AsSingleton();
            builder.Bind<Warrior>().AsSingleton();
            builder.Bind<Archer>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<Warrior>().Weapon, Is.InstanceOf<Sword>());
                Assert.Throws<BindingNotFoundException>(() => container.Resolve<Archer>());
            }
        }

        [Test]
        public void Resolve_AFailingPredicate_RejectsTheBinding()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().When(() => false).AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.Throws<BindingNotFoundException>(() => container.Resolve<IWeapon>());
            }
        }


        [Test]
        public void IfNotBound_WhenTheContractIsAlreadyBound_IsIgnored()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().AsSingleton();
            builder.Bind<IWeapon, Bow>().IfNotBound().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<IWeapon>(), Is.InstanceOf<Sword>());
            }
        }

        [Test]
        public void IfNotBound_WhenNothingIsBound_Applies()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Bow>().IfNotBound().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<IWeapon>(), Is.InstanceOf<Bow>());
            }
        }
    }
}

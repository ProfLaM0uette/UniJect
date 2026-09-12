using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ContainerIdTests
    {
        [Test]
        public void ResolveId_TwoIds_ReturnsTheMatchingConcreteType()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().WithId("melee").AsSingleton();
            builder.Bind<IWeapon, Bow>().WithId("ranged").AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.ResolveId<IWeapon>("melee"), Is.InstanceOf<Sword>());
                Assert.That(container.ResolveId<IWeapon>("ranged"), Is.InstanceOf<Bow>());
            }
        }

        [Test]
        public void Resolve_WithoutAnId_NeverSeesAnIdCarryingBinding()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().WithId("melee").AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.Throws<BindingNotFoundException>(() => container.Resolve<IWeapon>());
            }
        }

        [Test]
        public void Resolve_AnIdBindingAndAnIdLessOne_KeepsThemApartWhateverTheDeclarationOrder()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Bow>().WithId("ranged").AsSingleton();
            builder.Bind<IWeapon, Sword>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<IWeapon>(), Is.InstanceOf<Sword>());
                Assert.That(container.ResolveId<IWeapon>("ranged"), Is.InstanceOf<Bow>());
            }
        }


        [Test]
        public void HasBinding_AnIdCarryingBinding_IsFoundOnlyWithItsId()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().WithId("melee").AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.HasBinding<IWeapon>("melee"), Is.True);
                Assert.That(container.HasBinding<IWeapon>(), Is.False);
            }
        }
    }
}

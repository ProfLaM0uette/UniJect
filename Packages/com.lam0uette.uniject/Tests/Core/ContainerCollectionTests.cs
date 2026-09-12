using System.Collections.Generic;
using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ContainerCollectionTests
    {
        [Test]
        public void ResolveAll_SeveralBindingsOfOneContract_ReturnsThemInDeclarationOrder()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().AsSingleton();
            builder.Bind<IWeapon, Bow>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                IReadOnlyList<IWeapon> weapons = container.ResolveAll<IWeapon>();

                Assert.That(weapons.Count, Is.EqualTo(2));
                Assert.That(weapons[0], Is.InstanceOf<Sword>());
                Assert.That(weapons[1], Is.InstanceOf<Bow>());
            }
        }

        [Test]
        public void ResolveAll_NoBindingAtAll_ReturnsAnEmptyListInsteadOfThrowing()
        {
            ContainerBuilder builder = new ContainerBuilder();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.ResolveAll<IWeapon>(), Is.Empty);
            }
        }


        [Test]
        public void Resolve_AnEnumerableConstructorParameter_ReceivesEveryBinding()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().AsSingleton();
            builder.Bind<IWeapon, Bow>().AsSingleton();
            builder.Bind<Armoury>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<Armoury>().Weapons.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void Resolve_AnArrayContract_MaterializesAnArray()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<IWeapon[]>(), Is.InstanceOf<IWeapon[]>());
                Assert.That(container.Resolve<IWeapon[]>().Length, Is.EqualTo(1));
            }
        }

        [Test]
        public void Resolve_AReadOnlyListContract_IsSatisfiedByAnArray()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().AsSingleton();
            builder.Bind<IWeapon, Bow>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                IReadOnlyList<IWeapon> weapons = container.Resolve<IReadOnlyList<IWeapon>>();

                Assert.That(weapons.Count, Is.EqualTo(2));
                Assert.That(weapons, Is.InstanceOf<IWeapon[]>());
            }
        }

        [Test]
        public void Resolve_EveryInterfaceShapedCollection_AvoidsTheGenericListEntirely()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<IEnumerable<IWeapon>>(), Is.InstanceOf<IWeapon[]>());
                Assert.That(container.Resolve<IList<IWeapon>>(), Is.InstanceOf<IWeapon[]>());
                Assert.That(container.Resolve<ICollection<IWeapon>>(), Is.InstanceOf<IWeapon[]>());
            }
        }

        [Test]
        public void Resolve_AConcreteListContract_StillMaterializesARealList()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().AsSingleton();
            builder.Bind<IWeapon, Bow>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<List<IWeapon>>(), Is.InstanceOf<List<IWeapon>>());
                Assert.That(container.Resolve<List<IWeapon>>().Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void Resolve_ACollectionOfValueTypes_NeedsNoRuntimeGenericConstruction()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.BindInstance(7).WithId("a");

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<IEnumerable<int>>(), Is.InstanceOf<int[]>());
            }
        }

        [Test]
        public void Resolve_ACollectionWhoseMembersAreConditional_FiltersOnTheConsumer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Bind<IWeapon, Sword>().AsSingleton();
            builder.Bind<IWeapon, Bow>().WhenInjectedInto<Warrior>().AsSingleton();
            builder.Bind<Armoury>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<Armoury>().Weapons.Count, Is.EqualTo(1));
            }
        }
    }
}

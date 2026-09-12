using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ServiceExtensionsTests
    {
        [SetUp]
        public void ResetCounters()
        {
            MultiService.CreationCount = 0;
        }

        [Test]
        public void BindInterfacesTo_AClassWithSeveralInterfaces_SharesOneInstanceAcrossThemAll()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.BindInterfacesTo<MultiService>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                ILog asLog = container.Resolve<ILog>();
                ISaveable asSaveable = container.Resolve<ISaveable>();

                Assert.That(asSaveable, Is.SameAs(asLog));
                Assert.That(MultiService.CreationCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void BindInterfacesTo_TheConcreteTypeItself_IsNotResolvable()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.BindInterfacesTo<MultiService>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.Throws<BindingNotFoundException>(() => container.Resolve<MultiService>());
            }
        }

        [Test]
        public void BindInterfacesAndSelfTo_AlsoResolvesTheConcreteType_AsTheSameInstance()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.BindInterfacesAndSelfTo<MultiService>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<MultiService>(), Is.SameAs(container.Resolve<ILog>()));
                Assert.That(MultiService.CreationCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void BindInterfacesTo_AClassImplementingNothing_ThrowsAtBind()
        {
            ContainerBuilder builder = new ContainerBuilder();

            InvalidBindingException exception =
                Assert.Throws<InvalidBindingException>(() => builder.BindInterfacesTo<Standalone>());

            Assert.That(exception.Message, Does.Contain(nameof(Standalone)));
        }


        [Test]
        public void AddSingleton_WithAnImplementation_ResolvesItOnce()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.AddSingleton<ILog, MemoryLog>();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<ILog>(), Is.SameAs(container.Resolve<ILog>()));
            }
        }

        [Test]
        public void AddSingleton_WithAnInstance_HandsBackThatVeryInstance()
        {
            MemoryLog log = new MemoryLog();

            ContainerBuilder builder = new ContainerBuilder();
            builder.AddSingleton(log);

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<MemoryLog>(), Is.SameAs(log));
            }
        }

        [Test]
        public void AddSingleton_WithAFactory_CallsItExactlyOnce()
        {
            int calls = 0;

            ContainerBuilder builder = new ContainerBuilder();
            builder.AddSingleton<ILog>(resolver =>
            {
                calls++;
                return new MemoryLog();
            });

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                container.Resolve<ILog>();
                container.Resolve<ILog>();

                Assert.That(calls, Is.EqualTo(1));
            }
        }

        [Test]
        public void AddTransient_ResolvedTwice_ReturnsDistinctInstances()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.AddTransient<ILog, MemoryLog>();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<ILog>(), Is.Not.SameAs(container.Resolve<ILog>()));
            }
        }

        [Test]
        public void AddScoped_ResolvedFromTwoChildren_GivesEachItsOwn()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.AddScoped<ILog, MemoryLog>();

            using (DIContainer parent = builder.Build(ContainerOptions.Relaxed))
            {
                DIContainer first = parent.CreateChildBuilder().Build(ContainerOptions.Relaxed);
                DIContainer second = parent.CreateChildBuilder().Build(ContainerOptions.Relaxed);

                Assert.That(first.Resolve<ILog>(), Is.Not.SameAs(second.Resolve<ILog>()));
            }
        }

        [Test]
        public void TheFacade_IsChainable()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder
                .AddSingleton<ILog, MemoryLog>()
                .AddTransient<IWeapon, Sword>();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<ILog>(), Is.InstanceOf<MemoryLog>());
                Assert.That(container.Resolve<IWeapon>(), Is.InstanceOf<Sword>());
            }
        }


        [Test]
        public void TryAddSingleton_WhenTheContractIsAlreadyBound_IsIgnored()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.AddSingleton<IWeapon, Sword>();
            builder.TryAddSingleton<IWeapon, Bow>();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<IWeapon>(), Is.InstanceOf<Sword>());
            }
        }

        [Test]
        public void Replace_AnAlreadyDeclaredBinding_DropsTheFirstOneEntirely()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.AddSingleton<IWeapon, Sword>();
            builder.Replace<IWeapon, Bow>(Lifetime.Singleton);

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<IWeapon>(), Is.InstanceOf<Bow>());
            }
        }
    }
}

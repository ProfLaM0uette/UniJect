using NUnit.Framework;
using UnityEngine;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class MonoInstallerTests
    {
        private GameObject _host;

        [SetUp]
        public void CreateHost()
        {
            _host = new GameObject(nameof(MonoInstallerTests));
        }

        [TearDown]
        public void DestroyHost()
        {
            if (_host != null)
                Object.DestroyImmediate(_host);
        }

        [Test]
        public void Install_OnAMonoInstaller_DeclaresItsBindingsOnTheBuilder()
        {
            HelloInstaller installer = _host.AddComponent<HelloInstaller>();

            ContainerBuilder builder = new ContainerBuilder();
            builder.Install(installer);

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                Assert.That(container.Resolve<IGreeter>(), Is.InstanceOf<UnityGreeter>());
            }
        }

        [Test]
        public void IsEnabled_OnAnInactiveGameObject_IsFalse()
        {
            HelloInstaller installer = _host.AddComponent<HelloInstaller>();
            _host.SetActive(false);

            Assert.That(installer.IsEnabled, Is.False);
        }


        [Test]
        public void Inject_OnASceneBehaviour_FillsThePrivateInjectField()
        {
            HelloInstaller installer = _host.AddComponent<HelloInstaller>();
            GreetedBehaviour consumer = _host.AddComponent<GreetedBehaviour>();

            ContainerBuilder builder = new ContainerBuilder();
            builder.Install(installer);

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                container.Inject(consumer);

                Assert.That(consumer.Greeter, Is.Not.Null);
                Assert.That(consumer.Greeter.Greet(), Is.EqualTo("hello"));
            }
        }


        [Test]
        public void Dispose_AContainerBuiltFromAnInstaller_DestroysNothingItDoesNotOwn()
        {
            HelloInstaller installer = _host.AddComponent<HelloInstaller>();

            ContainerBuilder builder = new ContainerBuilder();
            builder.Install(installer);

            DIContainer container = builder.Build(UnityContainerOptions.Create());
            container.Resolve<IGreeter>();
            container.Dispose();

            Assert.That(_host, Is.Not.Null);
            Assert.That(installer, Is.Not.Null);
        }
    }
}

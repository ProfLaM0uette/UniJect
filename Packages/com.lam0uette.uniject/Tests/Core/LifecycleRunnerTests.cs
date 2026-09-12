using System.Collections.Generic;
using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class LifecycleRunnerTests
    {
        [Test]
        public void RunInitialize_SeveralInitializables_CallsThemInCreationOrder()
        {
            InitializationRecorder recorder = new InitializationRecorder();

            ContainerBuilder builder = new ContainerBuilder();
            builder.BindInstance(recorder);
            builder.Bind<FirstInitializable>().AsSingleton();
            builder.Bind<SecondInitializable>().AsSingleton().NonLazy();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                container.ResolveNonLazy();
                container.RunInitialize();

                Assert.That(
                    recorder.Order,
                    Is.EqualTo(new List<string> { nameof(FirstInitializable), nameof(SecondInitializable) }));
            }
        }

        [Test]
        public void RunInitialize_NothingResolvedYet_InitializesNothing()
        {
            InitializationRecorder recorder = new InitializationRecorder();

            ContainerBuilder builder = new ContainerBuilder();
            builder.BindInstance(recorder);
            builder.Bind<FirstInitializable>().AsSingleton();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                container.RunInitialize();
                Assert.That(recorder.Order, Is.Empty);
            }
        }

        [Test]
        public void RunInitialize_CalledTwice_NeverInitializesTheSameInstanceTwice()
        {
            InitializationRecorder recorder = new InitializationRecorder();

            ContainerBuilder builder = new ContainerBuilder();
            builder.BindInstance(recorder);
            builder.Bind<FirstInitializable>().AsSingleton().NonLazy();

            using (DIContainer container = builder.Build(ContainerOptions.Relaxed))
            {
                container.ResolveNonLazy();

                container.RunInitialize();
                container.RunInitialize();

                Assert.That(recorder.Order.Count, Is.EqualTo(1));
            }
        }
    }
}

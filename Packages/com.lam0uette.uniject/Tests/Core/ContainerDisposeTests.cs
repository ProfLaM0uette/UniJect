using System.Collections.Generic;
using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class ContainerDisposeTests
    {
        [Test]
        public void Dispose_OwnedSingletons_ReleasesInReverseCreationOrder()
        {
            DisposalRecorder recorder = new DisposalRecorder();

            ContainerBuilder builder = new ContainerBuilder();
            builder.BindInstance(recorder);
            builder.Bind<FirstDisposable>().AsSingleton();
            builder.Bind<SecondDisposable>().AsSingleton();

            DIContainer container = builder.Build(ContainerOptions.Relaxed);
            container.Resolve<SecondDisposable>();
            container.Dispose();

            Assert.That(
                recorder.Order,
                Is.EqualTo(new List<string> { nameof(SecondDisposable), nameof(FirstDisposable) }));
        }

        [Test]
        public void Dispose_BoundInstance_IsNeverDisposedByTheContainer()
        {
            DisposalRecorder recorder = new DisposalRecorder();
            FirstDisposable owned = new FirstDisposable(recorder);

            ContainerBuilder builder = new ContainerBuilder();
            builder.BindInstance(owned);

            DIContainer container = builder.Build(ContainerOptions.Relaxed);
            container.Resolve<FirstDisposable>();
            container.Dispose();

            Assert.That(recorder.Order, Is.Empty);
        }

        [Test]
        public void Dispose_CalledTwice_IsIdempotent()
        {
            DisposalRecorder recorder = new DisposalRecorder();

            ContainerBuilder builder = new ContainerBuilder();
            builder.BindInstance(recorder);
            builder.Bind<FirstDisposable>().AsSingleton();

            DIContainer container = builder.Build(ContainerOptions.Relaxed);
            container.Resolve<FirstDisposable>();

            container.Dispose();
            container.Dispose();

            Assert.That(recorder.Order.Count, Is.EqualTo(1));
        }
    }
}

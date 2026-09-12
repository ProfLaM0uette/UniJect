using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class LifecycleTickTests
    {
        [Test]
        public void Tick_ASingletonTickable_IsTickedOncePerCall()
        {
            TickRecorder recorder = new TickRecorder();
            DIContainer container = BuildWith(recorder, builder => builder.Bind<CountingTickable>().AsSingleton().NonLazy());

            using (container)
            {
                container.ResolveNonLazy();

                container.Tick(0.016f, null);
                container.Tick(0.016f, null);

                Assert.That(recorder.TickCount, Is.EqualTo(2));
            }
        }

        [Test]
        public void Tick_ATickableNeverResolved_IsNeverTicked()
        {
            TickRecorder recorder = new TickRecorder();
            DIContainer container = BuildWith(recorder, builder => builder.Bind<CountingTickable>().AsSingleton());

            using (container)
            {
                container.Tick(0.016f, null);
                Assert.That(recorder.TickCount, Is.EqualTo(0));
            }
        }

        [Test]
        public void Tick_AThrowingTickable_ReportsTheFailureWithoutStoppingTheOthers()
        {
            TickRecorder recorder = new TickRecorder();

            DIContainer container = BuildWith(recorder, builder =>
            {
                builder.Bind<ThrowingTickable>().AsSingleton().NonLazy();
                builder.Bind<CountingTickable>().AsSingleton().NonLazy();
            });

            using (container)
            {
                container.ResolveNonLazy();

                List<Exception> failures = new List<Exception>();
                container.Tick(0.016f, failures.Add);

                Assert.That(failures.Count, Is.EqualTo(1));
                Assert.That(recorder.TickCount, Is.EqualTo(1));
                Assert.That(
                    recorder.Order,
                    Is.EqualTo(new List<string> { nameof(ThrowingTickable), nameof(CountingTickable) }));
            }
        }

        [Test]
        public void Tick_ATickableAddedDuringATick_OnlyRunsFromTheNextTick()
        {
            TickRecorder recorder = new TickRecorder();

            DIContainer container = BuildWith(recorder, builder =>
            {
                builder.Bind<CountingTickable>().AsSingleton().NonLazy();
                builder.Bind<LateRegisteredTickable>().AsSingleton();
            });

            using (container)
            {
                container.ResolveNonLazy();

                container.Tick(0.016f, null);
                Assert.That(recorder.Order, Is.EqualTo(new List<string> { nameof(CountingTickable) }));

                container.Resolve<LateRegisteredTickable>();
                recorder.Order.Clear();

                container.Tick(0.016f, null);
                Assert.That(
                    recorder.Order,
                    Is.EqualTo(new List<string> { nameof(CountingTickable), nameof(LateRegisteredTickable) }));
            }
        }

        [Test]
        public void Tick_ATickableRemovedFromTheRunner_StopsTicking()
        {
            TickRecorder recorder = new TickRecorder();
            DIContainer container = BuildWith(recorder, builder => builder.Bind<CountingTickable>().AsSingleton().NonLazy());

            using (container)
            {
                container.ResolveNonLazy();
                container.Tick(0.016f, null);

                container.UnregisterTickable(container.Resolve<CountingTickable>());
                container.Tick(0.016f, null);

                Assert.That(recorder.TickCount, Is.EqualTo(1));
            }
        }


        [Test]
        public void FixedTick_AndLateTick_RunOnTheirOwnPasses()
        {
            TickRecorder recorder = new TickRecorder();
            DIContainer container = BuildWith(recorder, builder => builder.Bind<CountingTickable>().AsSingleton().NonLazy());

            using (container)
            {
                container.ResolveNonLazy();

                container.FixedTick(0.02f, null);
                container.FixedTick(0.02f, null);
                container.LateTick(0.016f, null);

                Assert.That(recorder.TickCount, Is.EqualTo(0));
                Assert.That(recorder.FixedTickCount, Is.EqualTo(2));
                Assert.That(recorder.LateTickCount, Is.EqualTo(1));
            }
        }


        [Test]
        public void HasTickables_WithNothingResolved_IsFalse()
        {
            TickRecorder recorder = new TickRecorder();
            DIContainer container = BuildWith(recorder, builder => builder.Bind<CountingTickable>().AsSingleton());

            using (container)
            {
                Assert.That(container.HasTickables, Is.False);

                container.Resolve<CountingTickable>();
                Assert.That(container.HasTickables, Is.True);
            }
        }


        private static DIContainer BuildWith(TickRecorder recorder, Action<IContainerBuilder> declare)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.BindInstance(recorder);
            declare(builder);

            return builder.Build(ContainerOptions.Relaxed);
        }
    }
}

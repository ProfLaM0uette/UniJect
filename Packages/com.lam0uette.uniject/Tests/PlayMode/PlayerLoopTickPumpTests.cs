using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace LaM0uette.UniJect
{
    [TestFixture]
    public sealed class PlayerLoopTickPumpTests
    {
        private readonly List<DIContainer> _containers = new List<DIContainer>();

        [TearDown]
        public void CleanUp()
        {
            for (int i = _containers.Count - 1; i >= 0; i--)
                _containers[i].Dispose();

            _containers.Clear();
            PlayerLoopTickPump.Reset();
        }

        [UnityTest]
        public IEnumerator Tick_APureCSharpTickable_TicksOncePerFrameWithNoMonoBehaviour()
        {
            FrameTickRecorder recorder = new FrameTickRecorder();
            DIContainer container = Build(recorder);

            container.ResolveNonLazy();
            container.StartTicking();

            yield return null;
            int start = recorder.TickCount;

            yield return null;
            yield return null;
            yield return null;

            Assert.That(recorder.TickCount - start, Is.EqualTo(3));
            Assert.That(recorder.LateTickCount, Is.GreaterThan(0));
        }

        [UnityTest]
        public IEnumerator StopTicking_AfterAFewFrames_StopsTheTicks()
        {
            FrameTickRecorder recorder = new FrameTickRecorder();
            DIContainer container = Build(recorder);

            container.ResolveNonLazy();
            container.StartTicking();

            yield return null;
            container.StopTicking();

            int stopped = recorder.TickCount;

            yield return null;
            yield return null;

            Assert.That(recorder.TickCount, Is.EqualTo(stopped));
        }

        [UnityTest]
        public IEnumerator Dispose_AContainerThatWasTicking_StopsItWithoutTouchingThePump()
        {
            FrameTickRecorder recorder = new FrameTickRecorder();
            DIContainer container = Build(recorder);

            container.ResolveNonLazy();
            container.StartTicking();

            yield return null;

            container.Dispose();
            _containers.Remove(container);

            int stopped = recorder.TickCount;

            yield return null;
            yield return null;

            Assert.That(recorder.TickCount, Is.EqualTo(stopped));
            Assert.That(PlayerLoopTickPump.CountNodes(), Is.EqualTo(3));
        }


        [Test]
        public void StartTicking_Once_InsertsExactlyThreeNodes()
        {
            PlayerLoopTickPump.Reset();

            FrameTickRecorder recorder = new FrameTickRecorder();
            DIContainer container = Build(recorder);

            container.ResolveNonLazy();
            container.StartTicking();

            Assert.That(PlayerLoopTickPump.CountNodes(), Is.EqualTo(3));
        }

        [Test]
        public void Reset_AfterRegistration_RemovesExactlyOurThreeNodes()
        {
            PlayerLoopTickPump.Reset();

            FrameTickRecorder recorder = new FrameTickRecorder();
            DIContainer container = Build(recorder);

            container.ResolveNonLazy();
            container.StartTicking();

            PlayerLoopTickPump.Reset();

            Assert.That(PlayerLoopTickPump.CountNodes(), Is.EqualTo(0));
            Assert.That(PlayerLoopTickPump.IsRegistered, Is.False);
        }

        [Test]
        public void Reset_ThenRegisterAgainThreeTimes_NeverStacksExtraNodes()
        {
            for (int session = 0; session < 3; session++)
            {
                PlayerLoopTickPump.Reset();

                FrameTickRecorder recorder = new FrameTickRecorder();
                DIContainer container = Build(recorder);

                container.ResolveNonLazy();
                container.StartTicking();

                Assert.That(PlayerLoopTickPump.CountNodes(), Is.EqualTo(3));
                Assert.That(PlayerLoopTickPump.ContainerCount, Is.EqualTo(1));
            }
        }


        private DIContainer Build(FrameTickRecorder recorder)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.BindInstance(recorder);
            builder.Bind<FrameTickable>().AsSingleton().NonLazy();

            ContainerOptions options = UnityContainerOptions.Create();
            options.ValidateOnBuild = false;

            DIContainer container = builder.Build(options);
            _containers.Add(container);

            return container;
        }
    }
}

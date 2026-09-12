using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.LowLevel;

namespace LaM0uette.UniJect
{
    internal static class PlayerLoopTickPump
    {
        #region Statements

        private static readonly List<DIContainer> CONTAINERS = new List<DIContainer>();
        private static readonly List<DIContainer> SNAPSHOT = new List<DIContainer>();

        private static readonly ProfilerMarker TICK_MARKER = new ProfilerMarker("UniJect.Tick");
        private static readonly ProfilerMarker FIXED_MARKER = new ProfilerMarker("UniJect.FixedTick");
        private static readonly ProfilerMarker LATE_MARKER = new ProfilerMarker("UniJect.LateTick");

        private static readonly Action<Exception> LOG_FAILURE = LogFailure;

        private static bool _registered;

        public static int ContainerCount
        {
            get { return CONTAINERS.Count; }
        }

        public static bool IsRegistered
        {
            get { return _registered; }
        }

        #endregion

        #region Methods

        public static void Add(DIContainer container)
        {
            if (container == null || CONTAINERS.Contains(container))
                return;

            CONTAINERS.Add(container);
            EnsureRegistered();
        }

        public static void Remove(DIContainer container)
        {
            CONTAINERS.Remove(container);
        }

        public static void Reset()
        {
            CONTAINERS.Clear();
            SNAPSHOT.Clear();

            if (_registered)
                RemoveNodes();

            _registered = false;
        }

        public static int CountNodes()
        {
            return CountNodes(PlayerLoop.GetCurrentPlayerLoop());
        }


        private static void EnsureRegistered()
        {
            if (_registered)
                return;

            PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();

            Insert(ref loop, typeof(UnityEngine.PlayerLoop.Update),
                typeof(UnityEngine.PlayerLoop.Update.ScriptRunBehaviourUpdate), typeof(TickPhase), Tick);

            Insert(ref loop, typeof(UnityEngine.PlayerLoop.FixedUpdate),
                typeof(UnityEngine.PlayerLoop.FixedUpdate.ScriptRunBehaviourFixedUpdate), typeof(FixedTickPhase),
                FixedTick);

            Insert(ref loop, typeof(UnityEngine.PlayerLoop.PreLateUpdate),
                typeof(UnityEngine.PlayerLoop.PreLateUpdate.ScriptRunBehaviourLateUpdate), typeof(LateTickPhase),
                LateTick);

            PlayerLoop.SetPlayerLoop(loop);
            _registered = true;
        }

        private static void Insert(
            ref PlayerLoopSystem loop,
            Type parentType,
            Type anchorType,
            Type phaseType,
            PlayerLoopSystem.UpdateFunction callback)
        {
            if (loop.subSystemList == null)
                return;

            for (int i = 0; i < loop.subSystemList.Length; i++)
            {
                PlayerLoopSystem child = loop.subSystemList[i];

                if (child.type != parentType)
                {
                    Insert(ref child, parentType, anchorType, phaseType, callback);
                    loop.subSystemList[i] = child;

                    continue;
                }

                loop.subSystemList[i] = WithNodeAfter(child, anchorType, phaseType, callback);
                return;
            }
        }

        private static PlayerLoopSystem WithNodeAfter(
            PlayerLoopSystem parent,
            Type anchorType,
            Type phaseType,
            PlayerLoopSystem.UpdateFunction callback)
        {
            PlayerLoopSystem[] children = parent.subSystemList ?? new PlayerLoopSystem[0];
            List<PlayerLoopSystem> rebuilt = new List<PlayerLoopSystem>(children.Length + 1);

            bool inserted = false;

            for (int i = 0; i < children.Length; i++)
            {
                rebuilt.Add(children[i]);

                if (children[i].type != anchorType)
                    continue;

                rebuilt.Add(new PlayerLoopSystem { type = phaseType, updateDelegate = callback });
                inserted = true;
            }

            if (!inserted)
                rebuilt.Add(new PlayerLoopSystem { type = phaseType, updateDelegate = callback });

            parent.subSystemList = rebuilt.ToArray();
            return parent;
        }

        private static void RemoveNodes()
        {
            PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
            Strip(ref loop);

            PlayerLoop.SetPlayerLoop(loop);
        }

        private static void Strip(ref PlayerLoopSystem loop)
        {
            if (loop.subSystemList == null)
                return;

            List<PlayerLoopSystem> kept = new List<PlayerLoopSystem>(loop.subSystemList.Length);

            for (int i = 0; i < loop.subSystemList.Length; i++)
            {
                PlayerLoopSystem child = loop.subSystemList[i];

                if (IsOurs(child.type))
                    continue;

                Strip(ref child);
                kept.Add(child);
            }

            loop.subSystemList = kept.ToArray();
        }

        private static int CountNodes(PlayerLoopSystem loop)
        {
            if (loop.subSystemList == null)
                return 0;

            int count = 0;

            for (int i = 0; i < loop.subSystemList.Length; i++)
            {
                if (IsOurs(loop.subSystemList[i].type))
                {
                    count++;
                    continue;
                }

                count += CountNodes(loop.subSystemList[i]);
            }

            return count;
        }

        private static bool IsOurs(Type type)
        {
            return type == typeof(TickPhase) || type == typeof(FixedTickPhase) || type == typeof(LateTickPhase);
        }

        private static void Tick()
        {
            using (TICK_MARKER.Auto())
            {
                float deltaTime = Time.deltaTime;
                int count = Snapshot();

                for (int i = 0; i < count; i++)
                    SNAPSHOT[i].Tick(deltaTime, LOG_FAILURE);
            }
        }

        private static void FixedTick()
        {
            using (FIXED_MARKER.Auto())
            {
                float fixedDeltaTime = Time.fixedDeltaTime;
                int count = Snapshot();

                for (int i = 0; i < count; i++)
                    SNAPSHOT[i].FixedTick(fixedDeltaTime, LOG_FAILURE);
            }
        }

        private static void LateTick()
        {
            using (LATE_MARKER.Auto())
            {
                float deltaTime = Time.deltaTime;
                int count = Snapshot();

                for (int i = 0; i < count; i++)
                    SNAPSHOT[i].LateTick(deltaTime, LOG_FAILURE);
            }
        }

        private static int Snapshot()
        {
            SNAPSHOT.Clear();

            for (int i = CONTAINERS.Count - 1; i >= 0; i--)
            {
                if (CONTAINERS[i].IsDisposed)
                    CONTAINERS.RemoveAt(i);
            }

            SNAPSHOT.AddRange(CONTAINERS);
            return SNAPSHOT.Count;
        }

        private static void LogFailure(Exception exception)
        {
            Debug.LogException(exception);
        }

        #endregion
    }
}

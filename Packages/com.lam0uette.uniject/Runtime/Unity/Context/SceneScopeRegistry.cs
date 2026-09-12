using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace LaM0uette.UniJect
{
    internal static class SceneScopeRegistry
    {
        #region Statements

        private static readonly Dictionary<Scene, DIContainer> CONTAINERS = new Dictionary<Scene, DIContainer>();

        #endregion

        #region Methods

        public static void Register(Scene scene, DIContainer container)
        {
            CONTAINERS[scene] = container;
        }

        public static bool TryGet(Scene scene, out DIContainer container)
        {
            return CONTAINERS.TryGetValue(scene, out container);
        }

        public static void CollectAll(List<DIContainer> destination)
        {
            foreach (KeyValuePair<Scene, DIContainer> entry in CONTAINERS)
                destination.Add(entry.Value);
        }

        public static bool TryGetScene(DIContainer container, out Scene scene)
        {
            foreach (KeyValuePair<Scene, DIContainer> entry in CONTAINERS)
            {
                for (DIContainer current = container; current != null; current = current.Parent)
                {
                    if (!ReferenceEquals(entry.Value, current))
                        continue;

                    scene = entry.Key;
                    return true;
                }
            }

            scene = default;
            return false;
        }

        public static void Unregister(Scene scene)
        {
            CONTAINERS.Remove(scene);
        }

        public static void Reset()
        {
            CONTAINERS.Clear();
        }

        #endregion
    }
}

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

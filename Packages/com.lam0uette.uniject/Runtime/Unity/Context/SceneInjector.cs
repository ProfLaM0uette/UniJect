using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LaM0uette.UniJect
{
    internal static class SceneInjector
    {
        #region Statements

        private static readonly List<GameObject> ROOTS = new List<GameObject>();
        private static readonly List<MonoBehaviour> BEHAVIOURS = new List<MonoBehaviour>();

        #endregion

        #region Methods

        public static void Inject(DIContainer container, Scene scene, SceneInjectionMode mode, MonoBehaviour self)
        {
            if (mode == SceneInjectionMode.None)
                return;

            if (!scene.IsValid() || !scene.isLoaded)
                return;

            ROOTS.Clear();
            scene.GetRootGameObjects(ROOTS);

            for (int i = 0; i < ROOTS.Count; i++)
            {
                GameObject root = ROOTS[i];

                if (mode == SceneInjectionMode.MarkedRootsOnly && root.GetComponent<UniJectInjectRoot>() == null)
                    continue;

                BEHAVIOURS.Clear();
                root.GetComponentsInChildren(true, BEHAVIOURS);

                for (int j = 0; j < BEHAVIOURS.Count; j++)
                {
                    MonoBehaviour behaviour = BEHAVIOURS[j];

                    if (behaviour is null || ReferenceEquals(behaviour, self))
                        continue;

                    container.Inject(behaviour);
                }
            }

            ROOTS.Clear();
            BEHAVIOURS.Clear();
        }

        public static void Reset()
        {
            ROOTS.Clear();
            BEHAVIOURS.Clear();
        }

        #endregion
    }
}

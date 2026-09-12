using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LaM0uette.UniJect
{
    internal static class UnityObjectFinder
    {
        #region Statements

        private static readonly List<GameObject> ROOTS = new List<GameObject>();

        #endregion

        #region Methods

        public static Component FindInScene(Scene scene, Type componentType, bool includeInactive)
        {
            if (!scene.IsValid() || !scene.isLoaded)
                return null;

            ROOTS.Clear();
            scene.GetRootGameObjects(ROOTS);

            for (int i = 0; i < ROOTS.Count; i++)
            {
                Component found = FindOn(ROOTS[i], componentType, true, includeInactive);

                if (found != null)
                    return found;
            }

            ROOTS.Clear();
            return null;
        }

        public static Component FindOn(
            GameObject host,
            Type componentType,
            bool includeChildren,
            bool includeInactive)
        {
            if (host == null)
                return null;

            if (!includeChildren)
            {
                Component direct = host.GetComponent(componentType);
                return direct;
            }

            Component[] found = host.GetComponentsInChildren(componentType, includeInactive);
            return found.Length == 0 ? null : found[0];
        }

        public static void Reset()
        {
            ROOTS.Clear();
        }

        #endregion
    }
}

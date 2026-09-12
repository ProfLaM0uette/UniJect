using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LaM0uette.UniJect
{
    public static class ScenePreview
    {
        #region Methods

        public static List<SceneContext> CollectContexts()
        {
            List<SceneContext> contexts = new List<SceneContext>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (!scene.isLoaded)
                    continue;

                GameObject[] roots = scene.GetRootGameObjects();

                for (int j = 0; j < roots.Length; j++)
                    contexts.AddRange(roots[j].GetComponentsInChildren<SceneContext>(true));
            }

            return contexts;
        }

        public static DIContainer Build(SceneContext context, bool validateScopes)
        {
            MonoInstallerBase[] installers = context.GetComponents<MonoInstallerBase>();

            if (installers.Length == 0)
                return null;

            ContainerBuilder builder = new ContainerBuilder();

            for (int i = 0; i < installers.Length; i++)
                builder.Install(installers[i]);

            ContainerOptions options = ContainerOptions.Relaxed;
            options.ValidateScopes = validateScopes;
            options.Description = "SceneContext '" + context.gameObject.scene.name + "' (edit mode preview)";

            return builder.Build(options);
        }

        #endregion
    }
}

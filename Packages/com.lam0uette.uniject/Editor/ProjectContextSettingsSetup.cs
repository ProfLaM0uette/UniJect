using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LaM0uette.UniJect
{
    internal static class ProjectContextSettingsSetup
    {
        #region Statements

        private const string DEFAULT_PATH = "Assets/ProjectContextSettings.asset";

        #endregion

        #region Methods

        public static ProjectContextSettings Find()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + nameof(ProjectContextSettings));

            if (guids.Length == 0)
                return null;

            return AssetDatabase.LoadAssetAtPath<ProjectContextSettings>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        public static bool IsPreloaded(ProjectContextSettings settings)
        {
            if (settings == null)
                return false;

            Object[] preloaded = PlayerSettings.GetPreloadedAssets();

            for (int i = 0; i < preloaded.Length; i++)
            {
                if (preloaded[i] == settings)
                    return true;
            }

            return false;
        }

        public static ProjectContextSettings CreateAndPreload()
        {
            ProjectContextSettings settings = Find();

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<ProjectContextSettings>();

                AssetDatabase.CreateAsset(settings, AssetDatabase.GenerateUniqueAssetPath(DEFAULT_PATH));
                AssetDatabase.SaveAssets();
            }

            Preload(settings);
            return settings;
        }

        public static void Preload(ProjectContextSettings settings)
        {
            if (settings == null || IsPreloaded(settings))
                return;

            List<Object> preloaded = new List<Object>(PlayerSettings.GetPreloadedAssets());
            preloaded.Add(settings);

            PlayerSettings.SetPreloadedAssets(preloaded.ToArray());
            AssetDatabase.SaveAssets();
        }

        #endregion
    }
}

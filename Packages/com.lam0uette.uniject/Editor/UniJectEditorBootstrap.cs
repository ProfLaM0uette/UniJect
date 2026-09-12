using UnityEditor;

namespace LaM0uette.UniJect
{
    [InitializeOnLoad]
    internal static class UniJectEditorBootstrap
    {
        #region Statements

        static UniJectEditorBootstrap()
        {
            UnityContainerOptions.Observer = EditorResolutionObserver.Instance;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        #endregion

        #region Methods

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingPlayMode)
                EditorResolutionObserver.Instance.Clear();
        }

        #endregion
    }
}

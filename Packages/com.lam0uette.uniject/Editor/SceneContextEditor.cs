using UnityEditor;
using UnityEngine;

namespace LaM0uette.UniJect
{
    [CustomEditor(typeof(SceneContext))]
    internal sealed class SceneContextEditor : Editor
    {
        #region Methods

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (!GUILayout.Button("Validate now"))
                return;

            ValidationReport report = PlayModeValidationGate.ValidateOpenScenes();

            if (report.ErrorCount == 0)
            {
                Debug.Log(report);
                return;
            }

            Debug.LogError(report);
        }

        #endregion
    }
}

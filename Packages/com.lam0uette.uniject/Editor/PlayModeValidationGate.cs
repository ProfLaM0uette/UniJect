using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LaM0uette.UniJect
{
    [InitializeOnLoad]
    internal static class PlayModeValidationGate
    {
        #region Statements

        static PlayModeValidationGate()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        #endregion

        #region Methods

        public static ValidationReport ValidateOpenScenes()
        {
            List<ValidationIssue> issues = new List<ValidationIssue>();
            List<SceneContext> contexts = ScenePreview.CollectContexts();

            for (int i = 0; i < contexts.Count; i++)
            {
                DIContainer preview = null;

                try
                {
                    preview = ScenePreview.Build(contexts[i], UniJectEditorSettings.ValidateScopes);

                    if (preview != null)
                        issues.AddRange(preview.Validate().Issues);
                }
                catch (UniJectException exception)
                {
                    issues.Add(new ValidationIssue(
                        IssueCode.UJ001,
                        ValidationSeverity.Error,
                        exception.Message,
                        null,
                        BindingOrigin.Unknown));
                }
                finally
                {
                    preview?.Dispose();
                }
            }

            return new ValidationReport(issues);
        }


        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingEditMode)
                return;

            if (!UniJectEditorSettings.BlockPlayModeOnError)
                return;

            ValidationReport report = ValidateOpenScenes();

            if (report.ErrorCount == 0)
                return;

            Debug.LogError(report);
            EditorApplication.isPlaying = false;
        }

        #endregion
    }
}

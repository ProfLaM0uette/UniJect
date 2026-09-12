using UnityEditor;

namespace LaM0uette.UniJect
{
    internal static class UniJectEditorSettings
    {
        #region Statements

        private const string BLOCK_KEY = "UniJect.BlockPlayModeOnValidationError";
        private const string VALIDATE_SCOPES_KEY = "UniJect.ValidateScopes";

        public static bool BlockPlayModeOnError
        {
            get { return EditorPrefs.GetBool(BLOCK_KEY, true); }
            set { EditorPrefs.SetBool(BLOCK_KEY, value); }
        }

        public static bool ValidateScopes
        {
            get { return EditorPrefs.GetBool(VALIDATE_SCOPES_KEY, true); }
            set { EditorPrefs.SetBool(VALIDATE_SCOPES_KEY, value); }
        }

        #endregion
    }
}

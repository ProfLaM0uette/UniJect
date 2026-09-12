using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace LaM0uette.UniJect
{
    internal static class UniJectSettingsProvider
    {
        #region Statements

        private const string PATH = "Project/UniJect";

        #endregion

        #region Methods

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            SettingsProvider provider = new SettingsProvider(PATH, SettingsScope.Project)
            {
                label = "UniJect",
                activateHandler = Populate,
                keywords = new HashSet<string> { "uniject", "dependency", "injection", "container" }
            };

            return provider;
        }


        private static void Populate(string searchContext, VisualElement root)
        {
            root.style.paddingLeft = 10;
            root.style.paddingTop = 8;

            Toggle block = new Toggle("Block play mode on validation error")
            {
                value = UniJectEditorSettings.BlockPlayModeOnError
            };

            block.RegisterValueChangedCallback(changed =>
                UniJectEditorSettings.BlockPlayModeOnError = changed.newValue);

            Toggle scopes = new Toggle("Check captive dependencies")
            {
                value = UniJectEditorSettings.ValidateScopes
            };

            scopes.RegisterValueChangedCallback(changed => UniJectEditorSettings.ValidateScopes = changed.newValue);

            root.Add(block);
            root.Add(scopes);
        }

        #endregion
    }
}

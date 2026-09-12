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
            root.Add(BuildProjectContextSection());
        }

        private static VisualElement BuildProjectContextSection()
        {
            VisualElement section = new VisualElement();
            section.style.marginTop = 14;

            Label title = new Label("Project context");
            title.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;
            section.Add(title);

            Label state = new Label();
            state.style.marginTop = 4;
            state.style.marginBottom = 4;
            section.Add(state);

            Button create = new Button { text = "Create and preload the settings asset" };
            section.Add(create);

            create.clicked += () =>
            {
                ProjectContextSettingsSetup.CreateAndPreload();
                Describe(state, create);
            };

            Describe(state, create);
            return section;
        }

        private static void Describe(Label state, Button create)
        {
            ProjectContextSettings settings = ProjectContextSettingsSetup.Find();

            if (settings == null)
            {
                state.text = "No ProjectContextSettings asset yet — project-scope installers cannot run.";
                create.text = "Create and preload the settings asset";
                create.SetEnabled(true);

                return;
            }

            if (!ProjectContextSettingsSetup.IsPreloaded(settings))
            {
                state.text = "The asset exists but is not in Preloaded Assets, so it is never loaded.";
                create.text = "Add it to Preloaded Assets";
                create.SetEnabled(true);

                return;
            }

            state.text = "Ready: the settings asset exists and is preloaded.";
            create.text = "Nothing to do";
            create.SetEnabled(false);
        }

        #endregion
    }
}

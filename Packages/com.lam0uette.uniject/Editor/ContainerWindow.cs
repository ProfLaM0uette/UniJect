using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LaM0uette.UniJect
{
    public sealed class ContainerWindow : EditorWindow
    {
        #region Statements

        private const string MENU_PATH = "Window/UniJect/Container";

        private ContainerWindowModel _model;
        private MultiColumnListView _list;
        private Label _source;

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            root.style.paddingLeft = 6;
            root.style.paddingRight = 6;
            root.style.paddingTop = 6;

            Toolbar toolbar = new Toolbar();
            toolbar.Add(new ToolbarButton(Refresh) { text = "Refresh" });
            root.Add(toolbar);

            _source = new Label();
            _source.style.marginTop = 4;
            _source.style.marginBottom = 4;
            root.Add(_source);

            _list = BuildList();
            _list.style.flexGrow = 1;
            root.Add(_list);

            Refresh();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        #endregion

        #region Methods

        [MenuItem(MENU_PATH)]
        public static void Open()
        {
            ContainerWindow window = GetWindow<ContainerWindow>();
            window.titleContent = new GUIContent("UniJect Container");
            window.Show();
        }

        public void Refresh()
        {
            _model = ContainerWindowModel.Collect();

            if (_source != null)
                _source.text = _model.Source;

            if (_list == null)
                return;

            _list.itemsSource = new List<ContainerBindingRow>(_model.Rows);
            _list.RefreshItems();
        }


        private MultiColumnListView BuildList()
        {
            MultiColumnListView view = new MultiColumnListView();

            AddColumn(view, "contract", 160, row => row.Contract);
            AddColumn(view, "concrete", 160, row => row.Concrete);
            AddColumn(view, "lifetime", 80, row => row.Lifetime);
            AddColumn(view, "id", 70, row => row.Id);
            AddColumn(view, "condition", 170, row => row.Condition);
            AddColumn(view, "non-lazy", 70, row => row.NonLazy ? "yes" : string.Empty);
            AddColumn(view, "state", 90, row => row.State);
            AddColumn(view, "injections", 70, row => row.InjectionCount.ToString());
            AddColumn(view, "origin", 180, row => row.Origin.ToString());

            view.selectionChanged += OnSelectionChanged;

            return view;
        }

        private void AddColumn(
            MultiColumnListView view,
            string title,
            int width,
            System.Func<ContainerBindingRow, string> read)
        {
            Column column = new Column
            {
                title = title,
                width = width,
                makeCell = () => new Label(),
                bindCell = (element, index) =>
                {
                    List<ContainerBindingRow> rows = (List<ContainerBindingRow>)view.itemsSource;
                    ((Label)element).text = read(rows[index]);
                }
            };

            view.columns.Add(column);
        }

        private void OnSelectionChanged(IEnumerable<object> selection)
        {
            foreach (object selected in selection)
            {
                if (!(selected is ContainerBindingRow row) || !row.Origin.IsKnown)
                    continue;

                InternalEditorOpen(row.Origin);
                return;
            }
        }

        private static void InternalEditorOpen(BindingOrigin origin)
        {
            string relative = ToProjectRelative(origin.SourceFile);

            if (relative == null)
                return;

            Object asset = AssetDatabase.LoadAssetAtPath<Object>(relative);

            if (asset != null)
                AssetDatabase.OpenAsset(asset, origin.SourceLine);
        }

        private static string ToProjectRelative(string absolute)
        {
            if (string.IsNullOrEmpty(absolute))
                return null;

            string normalised = absolute.Replace('\\', '/');
            string root = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);

            return normalised.StartsWith(root, System.StringComparison.OrdinalIgnoreCase)
                ? normalised.Substring(root.Length)
                : null;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode || change == PlayModeStateChange.EnteredEditMode)
                Refresh();
        }

        #endregion
    }
}

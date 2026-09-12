using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LaM0uette.UniJect
{
    public sealed class ContainerWindowModel
    {
        #region Statements

        private const string NOT_CREATED = "not created";
        private const string CREATED = "created";

        private readonly List<ContainerBindingRow> _rows;

        public IReadOnlyList<ContainerBindingRow> Rows
        {
            get { return _rows; }
        }

        public string Source { get; }

        private ContainerWindowModel(string source, List<ContainerBindingRow> rows)
        {
            Source = source;
            _rows = rows;
        }

        #endregion

        #region Methods

        public static ContainerWindowModel Empty(string source)
        {
            return new ContainerWindowModel(source, new List<ContainerBindingRow>());
        }

        public static ContainerWindowModel FromContainer(
            string source,
            DIContainer container,
            EditorResolutionObserver observer)
        {
            List<ContainerBindingRow> rows = new List<ContainerBindingRow>();

            if (container == null)
                return new ContainerWindowModel(source, rows);

            IReadOnlyList<Registration> registrations = container.Registrations;

            for (int i = 0; i < registrations.Count; i++)
                rows.Add(ToRow(registrations[i], observer));

            return new ContainerWindowModel(source, rows);
        }

        public static ContainerWindowModel Collect()
        {
            if (Application.isPlaying)
                return FromLiveContainers();

            return FromSceneInstallers();
        }


        private static ContainerWindowModel FromLiveContainers()
        {
            List<DIContainer> containers = new List<DIContainer>();

            if (ProjectContext.Exists)
                containers.Add(ProjectContext.Container);

            SceneScopeRegistry.CollectAll(containers);

            if (containers.Count == 0)
                return Empty("play mode — no container built yet");

            List<ContainerBindingRow> rows = new List<ContainerBindingRow>();

            for (int i = 0; i < containers.Count; i++)
            {
                IReadOnlyList<Registration> registrations = containers[i].Registrations;

                for (int j = 0; j < registrations.Count; j++)
                    rows.Add(ToRow(registrations[j], EditorResolutionObserver.Instance));
            }

            return new ContainerWindowModel("play mode — " + containers.Count + " live container(s)", rows);
        }

        private static ContainerWindowModel FromSceneInstallers()
        {
            List<ContainerBindingRow> rows = new List<ContainerBindingRow>();
            int contexts = 0;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (!scene.isLoaded)
                    continue;

                contexts += CollectScene(scene, rows);
            }

            if (contexts == 0)
                return Empty("edit mode — no SceneContext in the open scenes");

            return new ContainerWindowModel("edit mode — preview of " + contexts + " SceneContext(s)", rows);
        }

        private static int CollectScene(Scene scene, List<ContainerBindingRow> rows)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            int contexts = 0;

            for (int i = 0; i < roots.Length; i++)
            {
                SceneContext[] found = roots[i].GetComponentsInChildren<SceneContext>(true);

                for (int j = 0; j < found.Length; j++)
                {
                    if (!TryPreview(found[j], rows))
                        continue;

                    contexts++;
                }
            }

            return contexts;
        }

        private static bool TryPreview(SceneContext context, List<ContainerBindingRow> rows)
        {
            MonoInstallerBase[] installers = context.GetComponents<MonoInstallerBase>();

            if (installers.Length == 0)
                return false;

            ContainerBuilder builder = new ContainerBuilder();

            for (int i = 0; i < installers.Length; i++)
                builder.Install(installers[i]);

            DIContainer preview = builder.Build(ContainerOptions.Relaxed);

            try
            {
                IReadOnlyList<Registration> registrations = preview.Registrations;

                for (int i = 0; i < registrations.Count; i++)
                    rows.Add(ToRow(registrations[i], null));
            }
            finally
            {
                preview.Dispose();
            }

            return true;
        }

        private static ContainerBindingRow ToRow(Registration registration, EditorResolutionObserver observer)
        {
            int count = observer == null ? 0 : observer.CountFor(registration);

            return new ContainerBindingRow(
                Describe(registration.ContractTypes),
                registration.ConcreteType == null ? "<none>" : registration.ConcreteType.Name,
                registration.Lifetime.ToString(),
                registration.Id == null ? string.Empty : registration.Id.ToString(),
                registration.Condition == null ? string.Empty : registration.Condition.Describe(),
                registration.NonLazy,
                registration.Origin,
                count > 0 ? CREATED : NOT_CREATED,
                count);
        }

        private static string Describe(IReadOnlyList<System.Type> contracts)
        {
            if (contracts.Count == 1)
                return contracts[0].Name;

            string text = string.Empty;

            for (int i = 0; i < contracts.Count; i++)
            {
                if (i > 0)
                    text += ", ";

                text += contracts[i].Name;
            }

            return text;
        }

        #endregion
    }
}

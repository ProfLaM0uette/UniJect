using UnityEngine;

namespace LaM0uette.UniJect
{
    [DefaultExecutionOrder(ContextExecutionOrder.SCENE_CONTEXT)]
    [DisallowMultipleComponent]
    public sealed class SceneContext : MonoBehaviour
    {
        #region Statements

        [SerializeField] private MonoInstallerBase[] _installers;
        [SerializeField] private SceneInjectionMode _injectionMode = SceneInjectionMode.WholeScene;
        [SerializeField] private SceneContext _parentContext;

        public DIContainer Container { get; private set; }

        private void Awake()
        {
            MainThreadGuard.Capture();

            DIContainer parent = ResolveParentContainer();
            ContainerBuilder builder = (ContainerBuilder)parent.CreateChildBuilder();

            RunInstallers(builder);
            Container = builder.Build(UnityContainerOptions.Create());

            SceneScopeRegistry.Register(gameObject.scene, Container);

            Container.ResolveNonLazy();
            SceneInjector.Inject(Container, gameObject.scene, _injectionMode, this);
            Container.RunInitialize();
            Container.StartTicking();
        }

        private void OnDestroy()
        {
            if (Container == null)
                return;

            Container.StopTicking();
            SceneScopeRegistry.Unregister(gameObject.scene);

            Container.Dispose();
            Container = null;
        }

        #endregion

        #region Methods

        private DIContainer ResolveParentContainer()
        {
            if (_parentContext != null && _parentContext.Container != null)
                return _parentContext.Container;

            return ProjectContext.Container;
        }

        private void RunInstallers(ContainerBuilder builder)
        {
            if (_installers == null)
                return;

            for (int i = 0; i < _installers.Length; i++)
            {
                MonoInstallerBase installer = _installers[i];

                if (installer == null || !installer.IsEnabled)
                    continue;

                builder.Install(installer);
            }
        }

        #endregion
    }
}

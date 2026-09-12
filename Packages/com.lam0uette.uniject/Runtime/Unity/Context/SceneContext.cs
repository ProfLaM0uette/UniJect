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

        public DIContainer Container { get; private set; }

        private void Awake()
        {
            MainThreadGuard.Capture();

            ContainerBuilder builder = new ContainerBuilder();
            RunInstallers(builder);

            Container = builder.Build(UnityContainerOptions.Create());
            Container.ResolveNonLazy();

            SceneInjector.Inject(Container, gameObject.scene, _injectionMode, this);
        }

        private void OnDestroy()
        {
            Container?.Dispose();
            Container = null;
        }


        #endregion

        #region Methods

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

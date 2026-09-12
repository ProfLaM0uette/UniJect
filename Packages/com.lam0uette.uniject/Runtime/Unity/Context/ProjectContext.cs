using System.Collections.Generic;
using UnityEngine;

namespace LaM0uette.UniJect
{
    public static class ProjectContext
    {
        #region Statements

        private const string HOLDER_NAME = "[UniJect]";

        private static DIContainer _container;
        private static GameObject _holder;

        public static DIContainer Container
        {
            get { return _container ?? Create(); }
        }

        public static bool Exists
        {
            get { return _container != null; }
        }

        internal static Transform Holder
        {
            get { return _holder == null ? null : _holder.transform; }
        }

        #endregion

        #region Methods

        internal static void Initialize()
        {
            Create();
        }

        internal static void Reset()
        {
            _container = null;
            _holder = null;
        }


        private static DIContainer Create()
        {
            if (_container != null)
                return _container;

            MainThreadGuard.Capture();

            ContainerBuilder builder = new ContainerBuilder();
            RunSettingsInstallers(builder);

            _container = builder.Build(UnityContainerOptions.Create());

            if (Application.isPlaying)
            {
                _holder = new GameObject(HOLDER_NAME) { hideFlags = HideFlags.HideAndDontSave };
                Object.DontDestroyOnLoad(_holder);
                Application.quitting += Dispose;
            }

            _container.ResolveNonLazy();
            _container.RunInitialize();

            if (Application.isPlaying)
                _container.StartTicking();

            return _container;
        }

        private static void RunSettingsInstallers(ContainerBuilder builder)
        {
            ProjectContextSettings[] settings = Resources.FindObjectsOfTypeAll<ProjectContextSettings>();

            for (int i = 0; i < settings.Length; i++)
            {
                IReadOnlyList<ScriptableObjectInstaller> installers = settings[i].Installers;

                for (int j = 0; j < installers.Count; j++)
                {
                    if (installers[j] != null)
                        builder.Install(installers[j]);
                }
            }
        }

        private static void Dispose()
        {
            Application.quitting -= Dispose;

            _container?.Dispose();
            _container = null;

            if (_holder != null)
                Object.Destroy(_holder);

            _holder = null;
        }

        #endregion
    }
}

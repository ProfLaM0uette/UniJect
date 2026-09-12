using UnityEngine;

namespace LaM0uette.UniJect
{
    public abstract class MonoInstallerBase : MonoBehaviour, IInstaller
    {
        protected IContainerBuilder Container { get; private set; }

        public virtual bool IsEnabled
        {
            get { return isActiveAndEnabled; }
        }

        public virtual void Install()
        {
        }

        #region IInstaller

        void IInstaller.Install(IContainerBuilder builder)
        {
            Container = builder;
            Install();
        }

        #endregion
    }
}

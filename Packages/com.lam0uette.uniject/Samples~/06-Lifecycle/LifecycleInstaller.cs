namespace LaM0uette.UniJect.Samples.Lifecycle
{
    public sealed class LifecycleInstaller : MonoInstaller
    {
        #region Methods

        public override void Install()
        {
            Container.Bind<GameLoop>().AsSingleton().NonLazy();
        }

        #endregion
    }
}

namespace LaM0uette.UniJect.Samples.Installers
{
    public sealed class ScoreInstaller : MonoInstaller
    {
        #region Methods

        public override void Install()
        {
            Container.Bind<ScoreBoard>().AsSingleton().NonLazy();
        }

        #endregion
    }
}

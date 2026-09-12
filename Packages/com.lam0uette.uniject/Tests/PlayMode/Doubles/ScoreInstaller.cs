namespace LaM0uette.UniJect
{
    public sealed class ScoreInstaller : MonoInstaller
    {
        #region Methods

        public override void Install()
        {
            Container.Bind<IScore, SceneScore>().AsSingleton();
        }

        #endregion
    }
}

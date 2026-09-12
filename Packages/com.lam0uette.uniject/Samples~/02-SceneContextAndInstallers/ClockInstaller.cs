namespace LaM0uette.UniJect.Samples.Installers
{
    public sealed class ClockInstaller : MonoInstaller
    {
        #region Methods

        public override void Install()
        {
            Container.Bind<IClock, UnityClock>().AsSingleton();
        }

        #endregion
    }
}

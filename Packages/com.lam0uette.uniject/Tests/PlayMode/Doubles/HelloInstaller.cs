namespace LaM0uette.UniJect
{
    public sealed class HelloInstaller : MonoInstaller
    {
        #region Methods

        public override void Install()
        {
            Container.Bind<IGreeter, UnityGreeter>().AsSingleton();
        }

        #endregion
    }
}

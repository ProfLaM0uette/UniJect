namespace LaM0uette.UniJect.Samples.Hello
{
    public sealed class HelloInstaller : MonoInstaller
    {
        #region Methods

        public override void Install()
        {
            Container.Bind<IGreetingService, GreetingService>().AsSingleton();
        }

        #endregion
    }
}

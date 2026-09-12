using LaM0uette.UniJect;

namespace UniJect.Demo
{
    public sealed class DemoInstaller : MonoInstaller
    {
        #region Methods

        public override void Install()
        {
            Container.Bind<IDemoLogger, ConsoleDemoLogger>().AsSingleton();
            Container.Bind<IClock>().FromMethod(resolver => new DemoClock()).AsSingleton();

            Container.Bind<IWeapon, Sword>().AsSingleton();
            Container.Bind<IWeapon, Bow>().WhenInjectedInto<Archer>().AsSingleton();
            Container.Bind<IWeapon, Bow>().WithId("backup").AsSingleton();

            Container.Bind<Knight>().AsSingleton();
            Container.Bind<Archer>().AsSingleton();

            Container.Bind<IScoreRule, KillRule>().AsSingleton();
            Container.Bind<IScoreRule, TimeRule>().AsSingleton();
            Container.Bind<Referee>().AsSingleton();

            Container.Bind<GameClock>().AsSingleton().NonLazy();
        }

        #endregion
    }
}

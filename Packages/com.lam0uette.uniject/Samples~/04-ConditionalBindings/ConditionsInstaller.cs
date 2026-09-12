namespace LaM0uette.UniJect.Samples.Conditions
{
    public sealed class ConditionsInstaller : MonoInstaller
    {
        #region Methods

        public override void Install()
        {
            Container.Bind<IWeapon, Sword>().AsSingleton();
            Container.Bind<IWeapon, Bow>().WhenInjectedInto<Archer>().AsSingleton();
            Container.Bind<IWeapon, Bow>().WithId("backup").AsSingleton();

            Container.Bind<Archer>().AsSingleton().NonLazy();
        }

        #endregion
    }
}

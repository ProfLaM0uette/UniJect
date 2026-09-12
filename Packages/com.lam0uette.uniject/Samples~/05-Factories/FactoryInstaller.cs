namespace LaM0uette.UniJect.Samples.Factories
{
    public sealed class FactoryInstaller : MonoInstaller
    {
        #region Methods

        public override void Install()
        {
            Container.BindPlaceholderFactoryTo<BulletFactory, DoubleDamageFactory, Bullet, int>();
            Container.BindFactory<Bullet, int>();
        }

        #endregion
    }
}

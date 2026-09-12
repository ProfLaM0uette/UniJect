namespace LaM0uette.UniJect.Samples.Factories
{
    public sealed class DoubleDamageFactory : BulletFactory
    {
        #region Methods

        public override Bullet Create(int damage)
        {
            return base.Create(damage * 2);
        }

        #endregion
    }
}

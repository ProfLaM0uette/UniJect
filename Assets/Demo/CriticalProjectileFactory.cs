namespace UniJect.Demo
{
    public sealed class CriticalProjectileFactory : ProjectileFactory
    {
        #region Methods

        public override Projectile Create(int damage)
        {
            return base.Create(damage * 3);
        }

        #endregion
    }
}

namespace UniJect.Demo
{
    public sealed class Projectile
    {
        #region Statements

        public int Damage { get; }
        public IDemoLogger Logger { get; }

        public Projectile(int damage, IDemoLogger logger)
        {
            Damage = damage;
            Logger = logger;
        }

        #endregion
    }
}

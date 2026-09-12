namespace LaM0uette.UniJect.Samples.Factories
{
    public sealed class Bullet
    {
        #region Statements

        public int Damage { get; }

        public Bullet(int damage)
        {
            Damage = damage;
        }

        #endregion
    }
}

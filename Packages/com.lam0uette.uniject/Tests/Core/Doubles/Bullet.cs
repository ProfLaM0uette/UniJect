namespace LaM0uette.UniJect
{
    public sealed class Bullet
    {
        public int Damage { get; }
        public ILog Log { get; }

        public Bullet(int damage, ILog log)
        {
            Damage = damage;
            Log = log;
        }
    }
}

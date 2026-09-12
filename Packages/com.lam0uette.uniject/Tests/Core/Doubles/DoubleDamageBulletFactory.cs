namespace LaM0uette.UniJect
{
    public sealed class DoubleDamageBulletFactory : BulletFactory
    {
        public override Bullet Create(int damage)
        {
            return base.Create(damage * 2);
        }
    }
}

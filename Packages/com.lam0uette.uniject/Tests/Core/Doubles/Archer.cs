namespace LaM0uette.UniJect
{
    public sealed class Archer
    {
        public IWeapon Weapon { get; }

        public Archer(IWeapon weapon)
        {
            Weapon = weapon;
        }
    }
}

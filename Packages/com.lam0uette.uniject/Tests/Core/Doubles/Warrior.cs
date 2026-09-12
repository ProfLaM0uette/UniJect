namespace LaM0uette.UniJect
{
    public sealed class Warrior
    {
        public IWeapon Weapon { get; }

        public Warrior(IWeapon weapon)
        {
            Weapon = weapon;
        }
    }
}

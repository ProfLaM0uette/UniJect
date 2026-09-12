namespace UniJect.Demo
{
    public sealed class Archer
    {
        #region Statements

        public IWeapon Weapon { get; }

        public Archer(IWeapon weapon)
        {
            Weapon = weapon;
        }

        #endregion
    }
}

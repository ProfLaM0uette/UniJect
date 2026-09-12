namespace LaM0uette.UniJect.Samples.Conditions
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

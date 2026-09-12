namespace UniJect.Demo
{
    public sealed class Knight
    {
        #region Statements

        public IWeapon Weapon { get; }

        public Knight(IWeapon weapon)
        {
            Weapon = weapon;
        }

        #endregion
    }
}

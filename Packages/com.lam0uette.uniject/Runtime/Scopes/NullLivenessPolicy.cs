namespace LaM0uette.UniJect
{
    public sealed class NullLivenessPolicy : IInstanceLivenessPolicy
    {
        #region Statements

        public static readonly NullLivenessPolicy Instance = new NullLivenessPolicy();

        private NullLivenessPolicy()
        {
        }

        #endregion

        #region Methods

        public bool IsAlive(object instance)
        {
            return true;
        }

        #endregion
    }
}

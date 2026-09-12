namespace LaM0uette.UniJect
{
    internal sealed class TickRegistry : ITickRegistry
    {
        #region Statements

        public static readonly TickRegistry Instance = new TickRegistry();

        private TickRegistry()
        {
        }

        #endregion

        #region Methods

        public void Add(DIContainer container)
        {
            PlayerLoopTickPump.Add(container);
        }

        public void Remove(DIContainer container)
        {
            PlayerLoopTickPump.Remove(container);
        }

        #endregion
    }
}

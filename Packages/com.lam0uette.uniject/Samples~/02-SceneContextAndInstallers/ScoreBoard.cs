namespace LaM0uette.UniJect.Samples.Installers
{
    public sealed class ScoreBoard
    {
        #region Statements

        private readonly IClock _clock;

        public float StartedAt { get; }

        public ScoreBoard(IClock clock)
        {
            _clock = clock;
            StartedAt = clock.Now;
        }

        #endregion

        #region Methods

        public float Elapsed()
        {
            return _clock.Now - StartedAt;
        }

        #endregion
    }
}

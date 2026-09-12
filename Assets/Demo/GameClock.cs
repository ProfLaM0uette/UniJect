using System;
using LaM0uette.UniJect;

namespace UniJect.Demo
{
    public sealed class GameClock : IInitializable, ITickable, IDisposable
    {
        #region Statements

        private readonly IDemoLogger _logger;

        private float _elapsed;
        private int _ticks;

        public GameClock(IDemoLogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            _logger.Line("GameClock initialised before the first frame, with no MonoBehaviour.");
        }

        public void Tick(float deltaTime)
        {
            _elapsed += deltaTime;
            _ticks++;

            if (_ticks == 120)
                _logger.Line("GameClock ticked 120 times in " + _elapsed.ToString("0.00") + "s.");
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _logger.Line("GameClock disposed after " + _ticks + " ticks.");
        }

        #endregion
    }
}

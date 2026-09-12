namespace LaM0uette.UniJect
{
    public sealed class GameController
    {
        public PlayerService PlayerService { get; }

        public GameController(PlayerService playerService)
        {
            PlayerService = playerService;
        }
    }
}

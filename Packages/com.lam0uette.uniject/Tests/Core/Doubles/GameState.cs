namespace LaM0uette.UniJect
{
    public sealed class GameState
    {
        public static int CreationCount;

        public GameState()
        {
            CreationCount++;
        }
    }
}

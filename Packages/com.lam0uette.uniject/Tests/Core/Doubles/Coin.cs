namespace LaM0uette.UniJect
{
    public sealed class Coin
    {
        public static int CreationCount;

        public Coin()
        {
            CreationCount++;
        }
    }
}

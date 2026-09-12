namespace LaM0uette.UniJect
{
    public sealed class ScopedService
    {
        public static int CreationCount;

        public ScopedService()
        {
            CreationCount++;
        }
    }
}

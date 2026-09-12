namespace LaM0uette.UniJect
{
    public sealed class MutualB
    {
        [Inject] private MutualA _other;

        public MutualA Other
        {
            get { return _other; }
        }
    }
}

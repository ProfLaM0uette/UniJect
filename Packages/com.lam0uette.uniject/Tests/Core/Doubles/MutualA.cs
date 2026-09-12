namespace LaM0uette.UniJect
{
    public sealed class MutualA
    {
        [Inject] private MutualB _other;

        public MutualB Other
        {
            get { return _other; }
        }
    }
}

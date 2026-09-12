namespace LaM0uette.UniJect
{
    public sealed class Labelled
    {
        public IParams<string, int> Params { get; }

        public Labelled(IParams<string, int> parameters)
        {
            Params = parameters;
        }
    }
}

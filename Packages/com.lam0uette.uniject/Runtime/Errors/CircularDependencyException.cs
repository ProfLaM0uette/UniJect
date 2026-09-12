namespace LaM0uette.UniJect
{
    public sealed class CircularDependencyException : UniJectException
    {
        public ResolutionPath Path { get; }

        public CircularDependencyException(ResolutionPath path)
            : base(ResolutionMessage.CircularDependency(path))
        {
            Path = path;
        }
    }
}

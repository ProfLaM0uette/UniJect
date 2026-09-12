namespace LaM0uette.UniJect
{
    public sealed class ContainerPhaseException : UniJectException
    {
        public BuildPhase Actual { get; }
        public BuildPhase Required { get; }
        public string Operation { get; }

        public ContainerPhaseException(BuildPhase actual, BuildPhase required, string operation)
            : base(ResolutionMessage.ContainerPhase(actual, required, operation))
        {
            Actual = actual;
            Required = required;
            Operation = operation;
        }
    }
}

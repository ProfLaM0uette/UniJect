namespace LaM0uette.UniJect
{
    public sealed class ContainerValidationException : UniJectException
    {
        public ValidationReport Report { get; }

        public ContainerValidationException(ValidationReport report)
            : base(report.ToString())
        {
            Report = report;
        }
    }
}

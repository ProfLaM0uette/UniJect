using System;

namespace LaM0uette.UniJect
{
    public sealed class ValidationIssue
    {
        public string Code { get; }
        public ValidationSeverity Severity { get; }
        public string Message { get; }
        public Type ContractType { get; }
        public BindingOrigin Origin { get; }

        public ValidationIssue(
            string code,
            ValidationSeverity severity,
            string message,
            Type contractType,
            BindingOrigin origin)
        {
            Code = code;
            Severity = severity;
            Message = message;
            ContractType = contractType;
            Origin = origin;
        }

        public override string ToString()
        {
            return Code + " " + Severity + " at " + Origin + ": " + Message;
        }
    }
}

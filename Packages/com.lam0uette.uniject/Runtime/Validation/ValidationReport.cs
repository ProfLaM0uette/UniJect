using System.Collections.Generic;
using System.Text;

namespace LaM0uette.UniJect
{
    public sealed class ValidationReport
    {
        #region Statements

        public static readonly ValidationReport EMPTY = new ValidationReport(new List<ValidationIssue>());

        private readonly IReadOnlyList<ValidationIssue> _issues;

        public IReadOnlyList<ValidationIssue> Issues
        {
            get { return _issues; }
        }

        public int ErrorCount { get; }
        public int WarningCount { get; }

        public bool IsClean
        {
            get { return ErrorCount == 0 && WarningCount == 0; }
        }

        public ValidationReport(IReadOnlyList<ValidationIssue> issues)
        {
            _issues = issues;

            for (int i = 0; i < issues.Count; i++)
            {
                if (issues[i].Severity == ValidationSeverity.Error)
                    ErrorCount++;
                else if (issues[i].Severity == ValidationSeverity.Warning)
                    WarningCount++;
            }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            if (_issues.Count == 0)
                return "UniJect: no validation issues.";

            StringBuilder builder = new StringBuilder();
            builder.Append("UniJect: ").Append(ErrorCount).Append(" validation error(s), ")
                .Append(WarningCount).Append(" warning(s).");

            for (int i = 0; i < _issues.Count; i++)
                builder.AppendLine().Append("  ").Append(_issues[i]);

            return builder.ToString();
        }

        #endregion
    }
}

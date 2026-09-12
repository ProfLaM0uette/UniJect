using System;
using System.Text;

namespace LaM0uette.UniJect
{
    public sealed class DynamicCompositeCondition : IBindingCondition
    {
        #region Statements

        private readonly IBindingCondition[] _parts;

        public DynamicCompositeCondition(IBindingCondition[] parts)
        {
            _parts = parts ?? throw new ArgumentNullException(nameof(parts));
        }

        #endregion

        #region Methods

        public bool Matches(in ResolutionRequest request)
        {
            for (int i = 0; i < _parts.Length; i++)
            {
                if (!_parts[i].Matches(in request))
                    return false;
            }

            return true;
        }

        public string Describe()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < _parts.Length; i++)
            {
                if (i > 0)
                    builder.Append(" and ");

                builder.Append(_parts[i].Describe());
            }

            return builder.ToString();
        }

        #endregion
    }
}

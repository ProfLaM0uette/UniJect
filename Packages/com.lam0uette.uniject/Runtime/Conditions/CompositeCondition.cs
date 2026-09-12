using System;
using System.Text;

namespace LaM0uette.UniJect
{
    public sealed class CompositeCondition : IStaticCondition
    {
        #region Statements

        private readonly IBindingCondition[] _parts;

        public CompositeCondition(IBindingCondition[] parts)
        {
            _parts = parts ?? throw new ArgumentNullException(nameof(parts));
        }

        #endregion

        #region Methods

        public static IBindingCondition Create(IBindingCondition[] parts)
        {
            if (parts.Length == 1)
                return parts[0];

            for (int i = 0; i < parts.Length; i++)
            {
                if (!(parts[i] is IStaticCondition))
                    return new DynamicCompositeCondition(parts);
            }

            return new CompositeCondition(parts);
        }

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

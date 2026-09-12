using System;
using System.Text;

namespace LaM0uette.UniJect
{
    public sealed class InjectedIntoTypeCondition : IStaticCondition
    {
        #region Statements

        private readonly Type[] _types;

        public InjectedIntoTypeCondition(Type[] types)
        {
            _types = types ?? throw new ArgumentNullException(nameof(types));
        }

        #endregion

        #region Methods

        public bool Matches(in ResolutionRequest request)
        {
            return ConditionTypeMatch.Matches(_types, request.ConsumerType);
        }

        public string Describe()
        {
            return "WhenInjectedInto<" + ConditionTypeMatch.Describe(_types) + ">";
        }

        #endregion
    }
}

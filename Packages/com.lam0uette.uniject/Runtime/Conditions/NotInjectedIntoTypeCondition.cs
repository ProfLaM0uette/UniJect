using System;

namespace LaM0uette.UniJect
{
    public sealed class NotInjectedIntoTypeCondition : IStaticCondition
    {
        #region Statements

        private readonly Type[] _types;

        public NotInjectedIntoTypeCondition(Type[] types)
        {
            _types = types ?? throw new ArgumentNullException(nameof(types));
        }

        #endregion

        #region Methods

        public bool Matches(in ResolutionRequest request)
        {
            return !ConditionTypeMatch.Matches(_types, request.ConsumerType);
        }

        public string Describe()
        {
            return "WhenNotInjectedInto<" + ConditionTypeMatch.Describe(_types) + ">";
        }

        #endregion
    }
}

using System;

namespace LaM0uette.UniJect
{
    public sealed class PredicateCondition : IBindingCondition
    {
        #region Statements

        private readonly Func<ResolutionRequest, bool> _predicate;
        private readonly string _description;

        public PredicateCondition(Func<ResolutionRequest, bool> predicate, string description)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _description = description;
        }

        #endregion

        #region Methods

        public bool Matches(in ResolutionRequest request)
        {
            return _predicate(request);
        }

        public string Describe()
        {
            return _description;
        }

        #endregion
    }
}

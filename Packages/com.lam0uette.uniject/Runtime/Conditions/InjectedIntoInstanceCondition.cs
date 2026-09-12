using System;

namespace LaM0uette.UniJect
{
    public sealed class InjectedIntoInstanceCondition : IBindingCondition
    {
        #region Statements

        private readonly object _instance;

        public InjectedIntoInstanceCondition(object instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        #endregion

        #region Methods

        public bool Matches(in ResolutionRequest request)
        {
            return ReferenceEquals(request.ConsumerInstance, _instance);
        }

        public string Describe()
        {
            return "WhenInjectedIntoInstance(" + _instance.GetType().Name + ")";
        }

        #endregion
    }
}

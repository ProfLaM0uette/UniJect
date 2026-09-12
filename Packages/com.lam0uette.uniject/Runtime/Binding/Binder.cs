namespace LaM0uette.UniJect
{
    public sealed class Binder<TContract, TConcrete> where TConcrete : TContract
    {
        #region Statements

        public BindingDraft Draft { get; }

        public Binder(BindingDraft draft)
        {
            Draft = draft;
        }

        #endregion

        #region Methods

        public Binder<TContract, TConcrete> WhenInjectedInto<TConsumer>()
        {
            Draft.Conditions.Add(new InjectedIntoTypeCondition(new[] { typeof(TConsumer) }));
            return this;
        }

        public Binder<TContract, TConcrete> WhenNotInjectedInto<TConsumer>()
        {
            Draft.Conditions.Add(new NotInjectedIntoTypeCondition(new[] { typeof(TConsumer) }));
            return this;
        }

        #endregion
    }
}

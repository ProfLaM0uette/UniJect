namespace LaM0uette.UniJect
{
    public sealed class Binder<TContract, TConcrete> where TConcrete : TContract
    {
        public BindingDraft Draft { get; }

        public Binder(BindingDraft draft)
        {
            Draft = draft;
        }
    }
}

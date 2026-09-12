namespace LaM0uette.UniJect
{
    public static class BinderLifetimeExtensions
    {
        public static Binder<TContract, TConcrete> AsSingleton<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder)
            where TConcrete : TContract
        {
            binder.Draft.SetLifetime(Lifetime.Singleton);
            return binder;
        }

        public static Binder<TContract, TConcrete> AsTransient<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder)
            where TConcrete : TContract
        {
            binder.Draft.SetLifetime(Lifetime.Transient);
            return binder;
        }

        public static Binder<TContract, TConcrete> NonLazy<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder)
            where TConcrete : TContract
        {
            binder.Draft.NonLazy = true;
            return binder;
        }

        public static Binder<TContract, TConcrete> Lazy<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder)
            where TConcrete : TContract
        {
            binder.Draft.NonLazy = false;
            return binder;
        }
    }
}

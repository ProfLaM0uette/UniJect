namespace LaM0uette.UniJect
{
    internal sealed class CallSite
    {
        public Registration Registration { get; }
        public Lifetime Lifetime { get; }
        public CallSiteCacheLocation CacheLocation { get; }
        public int StoreSlot { get; }
        public IActivator Activator { get; }
        public bool IsCacheable { get; }

        public CallSite[] Dependencies { get; set; }

        public CallSite(Registration registration, bool isCacheable)
        {
            Registration = registration;
            Lifetime = registration.Lifetime;
            StoreSlot = registration.StoreSlot;
            Activator = registration.Activator;
            IsCacheable = isCacheable;
            CacheLocation = ToCacheLocation(registration.Lifetime);
        }

        private static CallSiteCacheLocation ToCacheLocation(Lifetime lifetime)
        {
            switch (lifetime)
            {
                case Lifetime.Singleton:
                    return CallSiteCacheLocation.Root;
                case Lifetime.Scoped:
                    return CallSiteCacheLocation.Scope;
                default:
                    return CallSiteCacheLocation.None;
            }
        }
    }
}

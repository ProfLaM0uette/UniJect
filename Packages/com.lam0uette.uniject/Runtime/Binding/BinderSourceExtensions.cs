using System;

namespace LaM0uette.UniJect
{
    public static class BinderSourceExtensions
    {
        public static Binder<TContract, TConcrete> FromNew<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder)
            where TConcrete : TContract
        {
            binder.Draft.SetDefaultSource();
            return binder;
        }

        public static Binder<TContract, TConcrete> FromInstance<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            TConcrete instance)
            where TConcrete : TContract
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            binder.Draft.SetSource(new InstanceActivatorSource(instance));
            return binder;
        }

        public static Binder<TContract, TConcrete> FromMethod<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            Func<TConcrete> factory)
            where TConcrete : TContract
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            binder.Draft.SetSource(new DelegateActivatorSource(resolver => factory()));
            return binder;
        }

        public static Binder<TContract, TConcrete> FromMethod<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            Func<IResolver, TConcrete> factory)
            where TConcrete : TContract
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            binder.Draft.SetSource(new DelegateActivatorSource(resolver => factory(resolver)));
            return binder;
        }

        public static Binder<TContract, TConcrete> FromResolve<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder)
            where TConcrete : TContract
        {
            binder.Draft.SetSource(new AliasActivatorSource(null));
            return binder;
        }
    }
}

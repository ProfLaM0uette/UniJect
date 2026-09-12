using System;

namespace LaM0uette.UniJect
{
    public static class BinderConditionExtensions
    {
        public static Binder<TContract, TConcrete> WithId<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            object id)
            where TConcrete : TContract
        {
            binder.Draft.Id = id;
            return binder;
        }

        public static Binder<TContract, TConcrete> When<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            Func<bool> predicate)
            where TConcrete : TContract
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            binder.Draft.Conditions.Add(new PredicateCondition(request => predicate(), "When(predicate)"));
            return binder;
        }

        public static Binder<TContract, TConcrete> When<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            Func<ResolutionRequest, bool> predicate)
            where TConcrete : TContract
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            binder.Draft.Conditions.Add(new PredicateCondition(predicate, "When(request => ...)"));
            return binder;
        }

        public static Binder<TContract, TConcrete> WhenInjectedInto<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            params Type[] types)
            where TConcrete : TContract
        {
            binder.Draft.Conditions.Add(new InjectedIntoTypeCondition(Require(types)));
            return binder;
        }

        public static Binder<TContract, TConcrete> WhenNotInjectedInto<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            params Type[] types)
            where TConcrete : TContract
        {
            binder.Draft.Conditions.Add(new NotInjectedIntoTypeCondition(Require(types)));
            return binder;
        }

        public static Binder<TContract, TConcrete> WhenInjectedIntoInstance<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            object instance)
            where TConcrete : TContract
        {
            binder.Draft.Conditions.Add(new InjectedIntoInstanceCondition(instance));
            return binder;
        }

        public static Binder<TContract, TConcrete> WhenNotInjectedIntoInstance<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            object instance)
            where TConcrete : TContract
        {
            binder.Draft.Conditions.Add(new NotInjectedIntoInstanceCondition(instance));
            return binder;
        }

        public static Binder<TContract, TConcrete> IfNotBound<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder)
            where TConcrete : TContract
        {
            binder.Draft.IfNotBound = true;
            return binder;
        }


        private static Type[] Require(Type[] types)
        {
            if (types == null || types.Length == 0)
                throw new ArgumentException("UniJect: the condition needs at least one type.", nameof(types));

            return types;
        }
    }
}

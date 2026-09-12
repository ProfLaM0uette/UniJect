using System;

namespace LaM0uette.UniJect
{
    public sealed class ContainerBindingRow
    {
        public string Contract { get; }
        public string Concrete { get; }
        public string Lifetime { get; }
        public string Id { get; }
        public string Condition { get; }
        public bool NonLazy { get; }
        public BindingOrigin Origin { get; }
        public string State { get; }
        public int InjectionCount { get; }

        public ContainerBindingRow(
            string contract,
            string concrete,
            string lifetime,
            string id,
            string condition,
            bool nonLazy,
            BindingOrigin origin,
            string state,
            int injectionCount)
        {
            Contract = contract;
            Concrete = concrete;
            Lifetime = lifetime;
            Id = id;
            Condition = condition;
            NonLazy = nonLazy;
            Origin = origin;
            State = state;
            InjectionCount = injectionCount;
        }

        public override string ToString()
        {
            return Contract + " -> " + Concrete + " (" + Lifetime + ") at " + Origin;
        }
    }
}

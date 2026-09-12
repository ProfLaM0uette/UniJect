using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class Registration
    {
        public IReadOnlyList<Type> ContractTypes { get; }
        public Type ConcreteType { get; }
        public Lifetime Lifetime { get; }
        public object Id { get; }
        public IActivator Activator { get; }
        public IBindingCondition Condition { get; }
        public bool NonLazy { get; }
        public BindingOrigin Origin { get; }

        internal int StoreSlot { get; set; }

        public Registration(
            IReadOnlyList<Type> contractTypes,
            Type concreteType,
            Lifetime lifetime,
            object id,
            IActivator activator,
            IBindingCondition condition,
            bool nonLazy,
            BindingOrigin origin)
        {
            if (contractTypes == null)
                throw new ArgumentNullException(nameof(contractTypes));

            if (activator == null)
                throw new ArgumentNullException(nameof(activator));

            ContractTypes = contractTypes;
            ConcreteType = concreteType;
            Lifetime = lifetime;
            Id = id;
            Activator = activator;
            Condition = condition;
            NonLazy = nonLazy;
            Origin = origin;
            StoreSlot = -1;
        }

        public override string ToString()
        {
            string concrete = ConcreteType == null ? "<unknown>" : ConcreteType.Name;
            return Id == null ? concrete : concrete + " (id: " + Id + ")";
        }
    }
}

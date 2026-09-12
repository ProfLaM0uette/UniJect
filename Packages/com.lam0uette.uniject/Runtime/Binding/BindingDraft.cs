using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class BindingDraft
    {
        #region Statements

        public List<Type> ContractTypes { get; } = new List<Type>();
        public Type ConcreteType { get; set; }
        public Lifetime Lifetime { get; set; }
        public object Id { get; set; }
        public bool NonLazy { get; set; }
        public IActivatorSource Source { get; set; }
        public List<IBindingCondition> Conditions { get; } = new List<IBindingCondition>();
        public BindingOrigin Origin { get; set; }

        internal int SourceAssignments { get; set; }
        internal bool LifetimeExplicit { get; set; }
        internal bool IfNotBound { get; set; }

        #endregion

        #region Methods

        public void SetSource(IActivatorSource source)
        {
            if (Source != null && !ReferenceEquals(Source, source))
                SourceAssignments++;

            Source = source;
        }

        public void SetDefaultSource()
        {
            if (Source == null || Source is ConstructorActivatorSource)
                Source = ConstructorActivatorSource.Instance;
        }

        public void SetLifetime(Lifetime lifetime)
        {
            Lifetime = lifetime;
            LifetimeExplicit = true;
        }

        #endregion
    }
}

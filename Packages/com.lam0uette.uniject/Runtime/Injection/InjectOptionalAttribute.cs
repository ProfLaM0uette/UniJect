using System;
using JetBrains.Annotations;

namespace LaM0uette.UniJect
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter,
        AllowMultiple = false)]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign | ImplicitUseKindFlags.Access)]
    public sealed class InjectOptionalAttribute : InjectAttributeBase
    {
        public object Id { get; }

        public InjectOptionalAttribute()
        {
        }

        public InjectOptionalAttribute(object id)
        {
            Id = id;
        }
    }
}

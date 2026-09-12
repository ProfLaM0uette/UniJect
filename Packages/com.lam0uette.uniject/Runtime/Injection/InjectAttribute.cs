using System;
using JetBrains.Annotations;

namespace LaM0uette.UniJect
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method |
        AttributeTargets.Constructor | AttributeTargets.Parameter,
        AllowMultiple = false)]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign | ImplicitUseKindFlags.Access)]
    public sealed class InjectAttribute : InjectAttributeBase
    {
        public object Id { get; }

        public InjectAttribute()
        {
        }

        public InjectAttribute(object id)
        {
            Id = id;
        }
    }
}

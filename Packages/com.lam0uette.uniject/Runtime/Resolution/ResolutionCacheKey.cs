using System;
using System.Runtime.CompilerServices;

namespace LaM0uette.UniJect
{
    internal readonly struct ResolutionCacheKey : IEquatable<ResolutionCacheKey>
    {
        public Type ContractType { get; }
        public object Id { get; }
        public Type ConsumerType { get; }

        public ResolutionCacheKey(Type contractType, object id, Type consumerType)
        {
            ContractType = contractType;
            Id = id;
            ConsumerType = consumerType;
        }

        public bool Equals(ResolutionCacheKey other)
        {
            return ReferenceEquals(ContractType, other.ContractType) &&
                   ReferenceEquals(ConsumerType, other.ConsumerType) &&
                   Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            return obj is ResolutionCacheKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            int hash = ContractType == null ? 0 : RuntimeHelpers.GetHashCode(ContractType);
            hash = (hash * 397) ^ (ConsumerType == null ? 0 : RuntimeHelpers.GetHashCode(ConsumerType));
            return Id == null ? hash : (hash * 397) ^ Id.GetHashCode();
        }
    }
}

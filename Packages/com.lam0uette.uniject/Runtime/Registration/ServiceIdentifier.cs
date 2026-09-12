using System;
using System.Runtime.CompilerServices;

namespace LaM0uette.UniJect
{
    public readonly struct ServiceIdentifier : IEquatable<ServiceIdentifier>
    {
        public Type ContractType { get; }
        public object Id { get; }

        public ServiceIdentifier(Type contractType, object id)
        {
            ContractType = contractType;
            Id = id;
        }

        public bool Equals(ServiceIdentifier other)
        {
            return ReferenceEquals(ContractType, other.ContractType) && Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            return obj is ServiceIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            int hash = ContractType == null ? 0 : RuntimeHelpers.GetHashCode(ContractType);
            return Id == null ? hash : (hash * 397) ^ Id.GetHashCode();
        }

        public override string ToString()
        {
            string contract = ContractType == null ? "<null>" : ContractType.Name;
            return Id == null ? contract : contract + " (id: " + Id + ")";
        }
    }
}

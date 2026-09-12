using System;

namespace LaM0uette.UniJect
{
    public sealed class ResolutionRecord
    {
        public Type ContractType { get; }
        public object Id { get; }
        public Type ConsumerType { get; }
        public Registration Registration { get; }

        public ResolutionRecord(Type contractType, object id, Type consumerType, Registration registration)
        {
            ContractType = contractType;
            Id = id;
            ConsumerType = consumerType;
            Registration = registration;
        }
    }
}

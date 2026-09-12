using System;

namespace LaM0uette.UniJect
{
    public sealed class DuplicateBindingException : UniJectException
    {
        public Type ContractType { get; }
        public object Id { get; }
        public Type ConcreteType { get; }
        public BindingOrigin First { get; }
        public BindingOrigin Second { get; }

        public DuplicateBindingException(
            Type contractType,
            object id,
            Type concreteType,
            BindingOrigin first,
            BindingOrigin second)
            : base(ResolutionMessage.DuplicateBinding(contractType, id, concreteType, first, second))
        {
            ContractType = contractType;
            Id = id;
            ConcreteType = concreteType;
            First = first;
            Second = second;
        }
    }
}

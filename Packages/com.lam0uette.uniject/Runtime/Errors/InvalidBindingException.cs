using System;

namespace LaM0uette.UniJect
{
    public sealed class InvalidBindingException : UniJectException
    {
        public string Code { get; }
        public Type ContractType { get; }
        public BindingOrigin Origin { get; }

        public InvalidBindingException(string code, Type contractType, BindingOrigin origin, string reason)
            : base(ResolutionMessage.InvalidBinding(code, contractType, origin, reason))
        {
            Code = code;
            ContractType = contractType;
            Origin = origin;
        }
    }
}

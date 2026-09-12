using System;

namespace LaM0uette.UniJect
{
    public sealed class NoSuitableConstructorException : UniJectException
    {
        public Type ConcreteType { get; }

        public NoSuitableConstructorException(Type concreteType)
            : base(ResolutionMessage.NoSuitableConstructor(concreteType))
        {
            ConcreteType = concreteType;
        }
    }
}

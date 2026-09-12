using System;

namespace LaM0uette.UniJect
{
    public sealed class ActivationException : UniJectException
    {
        public Type ConcreteType { get; }
        public ResolutionPath Path { get; }

        public ActivationException(Type concreteType, ResolutionPath path)
            : base(ResolutionMessage.Activation(concreteType, path, true))
        {
            ConcreteType = concreteType;
            Path = path;
        }

        public ActivationException(Type concreteType, ResolutionPath path, Exception innerException)
            : base(ResolutionMessage.Activation(concreteType, path, false), innerException)
        {
            ConcreteType = concreteType;
            Path = path;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;

namespace LaM0uette.UniJect
{
    public sealed class AmbiguousConstructorException : UniJectException
    {
        public Type ConcreteType { get; }
        public IReadOnlyList<ConstructorInfo> Candidates { get; }

        public AmbiguousConstructorException(Type concreteType, IReadOnlyList<ConstructorInfo> candidates)
            : base(ResolutionMessage.AmbiguousConstructor(concreteType, candidates))
        {
            ConcreteType = concreteType;
            Candidates = candidates;
        }
    }
}

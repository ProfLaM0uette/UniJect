using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class AmbiguousBindingException : UniJectException
    {
        public Type ContractType { get; }
        public object Id { get; }
        public ResolutionPath Path { get; }
        public IReadOnlyList<Registration> Candidates { get; }

        public AmbiguousBindingException(
            Type contractType,
            object id,
            ResolutionPath path,
            IReadOnlyList<Registration> candidates)
            : base(ResolutionMessage.AmbiguousBinding(contractType, id, path, candidates))
        {
            ContractType = contractType;
            Id = id;
            Path = path;
            Candidates = candidates;
        }
    }
}

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
            : this(contractType, id, path, candidates, null, null)
        {
        }

        internal AmbiguousBindingException(
            Type contractType,
            object id,
            ResolutionPath path,
            IReadOnlyList<Registration> candidates,
            string container,
            string installers)
            : base(ResolutionMessage.AmbiguousBinding(contractType, id, path, candidates, container, installers))
        {
            ContractType = contractType;
            Id = id;
            Path = path;
            Candidates = candidates;
        }
    }
}

using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class BindingNotFoundException : UniJectException
    {
        public Type ContractType { get; }
        public object Id { get; }
        public ResolutionPath Path { get; }
        public IReadOnlyList<SelectionFailure> NearMatches { get; }

        public BindingNotFoundException(
            Type contractType,
            object id,
            ResolutionPath path,
            IReadOnlyList<SelectionFailure> nearMatches)
            : base(ResolutionMessage.BindingNotFound(contractType, id, path, nearMatches))
        {
            ContractType = contractType;
            Id = id;
            Path = path;
            NearMatches = nearMatches;
        }
    }
}

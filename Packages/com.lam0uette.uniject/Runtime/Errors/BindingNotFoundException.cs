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
            : this(contractType, id, path, nearMatches, null, null)
        {
        }

        internal BindingNotFoundException(
            Type contractType,
            object id,
            ResolutionPath path,
            IReadOnlyList<SelectionFailure> nearMatches,
            string container,
            string installers)
            : base(ResolutionMessage.BindingNotFound(contractType, id, path, nearMatches, container, installers))
        {
            ContractType = contractType;
            Id = id;
            Path = path;
            NearMatches = nearMatches;
        }
    }
}

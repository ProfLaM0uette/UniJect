using System;

namespace LaM0uette.UniJect
{
    public readonly struct ResolutionRequest
    {
        public ServiceIdentifier Identifier { get; }
        public Type ConsumerType { get; }
        public object ConsumerInstance { get; }
        public string MemberName { get; }
        public InjectionSiteKind SiteKind { get; }

        public Type ContractType
        {
            get { return Identifier.ContractType; }
        }

        public object Id
        {
            get { return Identifier.Id; }
        }

        public ResolutionRequest(
            ServiceIdentifier identifier,
            Type consumerType,
            object consumerInstance,
            string memberName,
            InjectionSiteKind siteKind)
        {
            Identifier = identifier;
            ConsumerType = consumerType;
            ConsumerInstance = consumerInstance;
            MemberName = memberName;
            SiteKind = siteKind;
        }

        public static ResolutionRequest ForRoot(Type contractType, object id)
        {
            return new ResolutionRequest(
                new ServiceIdentifier(contractType, id),
                null,
                null,
                null,
                InjectionSiteKind.Constructor);
        }

        public ResolutionFrame ToFrame()
        {
            return new ResolutionFrame(Identifier, SiteKind, MemberName);
        }
    }
}

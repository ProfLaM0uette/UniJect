namespace LaM0uette.UniJect
{
    public readonly struct ResolutionFrame
    {
        public ServiceIdentifier Identifier { get; }
        public InjectionSiteKind SiteKind { get; }
        public string MemberName { get; }

        public ResolutionFrame(ServiceIdentifier identifier, InjectionSiteKind siteKind, string memberName)
        {
            Identifier = identifier;
            SiteKind = siteKind;
            MemberName = memberName;
        }

        public string DescribeSite()
        {
            if (MemberName == null)
                return null;

            switch (SiteKind)
            {
                case InjectionSiteKind.Constructor:
                    return "ctor parameter '" + MemberName + "'";
                case InjectionSiteKind.Field:
                    return "field '" + MemberName + "'";
                case InjectionSiteKind.Property:
                    return "property '" + MemberName + "'";
                case InjectionSiteKind.Method:
                    return "method parameter '" + MemberName + "'";
                default:
                    return "IInjectable parameter '" + MemberName + "'";
            }
        }

        public override string ToString()
        {
            string site = DescribeSite();
            return site == null ? Identifier.ToString() : Identifier + " (" + site + ")";
        }
    }
}

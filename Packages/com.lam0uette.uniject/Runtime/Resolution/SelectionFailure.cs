namespace LaM0uette.UniJect
{
    public sealed class SelectionFailure
    {
        public Registration Registration { get; }
        public string Reason { get; }

        public SelectionFailure(Registration registration, string reason)
        {
            Registration = registration;
            Reason = reason;
        }

        public override string ToString()
        {
            string origin = Registration == null ? "<unknown>" : Registration.Origin.ToString();
            return origin + " — rejected: " + Reason;
        }
    }
}

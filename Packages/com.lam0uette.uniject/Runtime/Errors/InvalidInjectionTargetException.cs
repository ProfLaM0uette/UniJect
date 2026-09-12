using System;

namespace LaM0uette.UniJect
{
    public sealed class InvalidInjectionTargetException : UniJectException
    {
        public Type DeclaringType { get; }
        public string MemberName { get; }
        public string Reason { get; }

        public InvalidInjectionTargetException(Type declaringType, string memberName, string reason)
            : base(ResolutionMessage.InvalidInjectionTarget(declaringType, memberName, reason))
        {
            DeclaringType = declaringType;
            MemberName = memberName;
            Reason = reason;
        }
    }
}

using System;

namespace JetBrains.Annotations
{
    [Flags]
    internal enum ImplicitUseTargetFlags
    {
        Itself = 1,
        Members = 2,
        WithInheritors = 4,
        WithMembers = Itself | Members
    }
}

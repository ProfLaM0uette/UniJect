using System;

namespace JetBrains.Annotations
{
    [Flags]
    internal enum ImplicitUseKindFlags
    {
        Access = 1,
        Assign = 2,
        InstantiatedWithFixedConstructorSignature = 4,
        InstantiatedNoFixedConstructorSignature = 8,
        Default = Access | Assign | InstantiatedWithFixedConstructorSignature
    }
}

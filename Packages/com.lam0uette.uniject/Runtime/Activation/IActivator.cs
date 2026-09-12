using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public interface IActivator
    {
        Type ProducedType { get; }
        Ownership Ownership { get; }
        IReadOnlyList<InjectionParameter> DeclaredDependencies { get; }

        object Create(in ResolutionContext context);

        void Inject(object instance, in ResolutionContext context);
    }
}

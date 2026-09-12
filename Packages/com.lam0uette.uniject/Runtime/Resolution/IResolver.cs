using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public interface IResolver
    {
        object Resolve(Type contractType);

        object ResolveId(Type contractType, object id);

        bool TryResolve(Type contractType, out object instance);

        bool TryResolveId(Type contractType, object id, out object instance);

        IReadOnlyList<object> ResolveAll(Type contractType);

        bool HasBinding(Type contractType, object id);
    }
}

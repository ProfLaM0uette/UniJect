using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public interface IInstantiator
    {
        object CreateInstance(Type concreteType);

        object CreateInstance(Type concreteType, IReadOnlyList<object> arguments);

        void Inject(object target);
    }
}

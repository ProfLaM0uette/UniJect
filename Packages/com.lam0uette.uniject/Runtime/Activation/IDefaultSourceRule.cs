using System;

namespace LaM0uette.UniJect
{
    public interface IDefaultSourceRule
    {
        IActivatorSource Resolve(Type concreteType);
    }
}

using System;

namespace LaM0uette.UniJect
{
    public interface IActivatorSource
    {
        IActivator Build(Type concreteType, IInjector injector);
    }
}

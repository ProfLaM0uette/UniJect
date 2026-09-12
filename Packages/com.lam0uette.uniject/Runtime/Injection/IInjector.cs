using System;

namespace LaM0uette.UniJect
{
    public interface IInjector
    {
        InjectionPlan GetPlan(Type type);

        object CreateInstance(Type concreteType, in ResolutionContext context);

        void Inject(object target, in ResolutionContext context);
    }
}

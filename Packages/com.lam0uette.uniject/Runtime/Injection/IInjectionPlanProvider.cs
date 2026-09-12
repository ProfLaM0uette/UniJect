using System;

namespace LaM0uette.UniJect
{
    public interface IInjectionPlanProvider
    {
        InjectionPlan GetPlan(Type type);
    }
}

using System.Reflection;

namespace LaM0uette.UniJect
{
    public sealed class PropertyInjectionSite
    {
        public MethodInfo Setter { get; }
        public InjectionParameter Parameter { get; }

        public PropertyInjectionSite(MethodInfo setter, InjectionParameter parameter)
        {
            Setter = setter;
            Parameter = parameter;
        }
    }
}

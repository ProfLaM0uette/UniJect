using System.Reflection;

namespace LaM0uette.UniJect
{
    public sealed class MethodInjectionSite
    {
        public MethodInfo Method { get; }
        public InjectionParameter[] Parameters { get; }

        public MethodInjectionSite(MethodInfo method, InjectionParameter[] parameters)
        {
            Method = method;
            Parameters = parameters;
        }
    }
}

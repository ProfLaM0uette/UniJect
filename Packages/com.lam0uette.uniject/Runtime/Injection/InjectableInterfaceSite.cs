using System.Reflection;

namespace LaM0uette.UniJect
{
    public sealed class InjectableInterfaceSite
    {
        public MethodInfo Method { get; }
        public InjectionParameter[] Parameters { get; }

        public InjectableInterfaceSite(MethodInfo method, InjectionParameter[] parameters)
        {
            Method = method;
            Parameters = parameters;
        }
    }
}

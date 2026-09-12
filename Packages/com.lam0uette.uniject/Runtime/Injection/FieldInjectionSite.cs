using System.Reflection;

namespace LaM0uette.UniJect
{
    public sealed class FieldInjectionSite
    {
        public FieldInfo Field { get; }
        public InjectionParameter Parameter { get; }

        public FieldInjectionSite(FieldInfo field, InjectionParameter parameter)
        {
            Field = field;
            Parameter = parameter;
        }
    }
}

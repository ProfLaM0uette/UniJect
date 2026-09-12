using System.Reflection;

namespace LaM0uette.UniJect
{
    public sealed class InjectionPlan
    {
        public static readonly InjectionPlan EMPTY = new InjectionPlan(
            null,
            new InjectionParameter[0],
            new FieldInjectionSite[0],
            new PropertyInjectionSite[0],
            new MethodInjectionSite[0],
            new InjectableInterfaceSite[0]);

        public ConstructorInfo Constructor { get; }
        public InjectionParameter[] ConstructorParameters { get; }
        public FieldInjectionSite[] Fields { get; }
        public PropertyInjectionSite[] Properties { get; }
        public MethodInjectionSite[] Methods { get; }
        public InjectableInterfaceSite[] InjectableInterfaces { get; }

        public bool HasMemberSites
        {
            get
            {
                return Fields.Length > 0 || Properties.Length > 0 || Methods.Length > 0 ||
                       InjectableInterfaces.Length > 0;
            }
        }

        public InjectionPlan(
            ConstructorInfo constructor,
            InjectionParameter[] constructorParameters,
            FieldInjectionSite[] fields,
            PropertyInjectionSite[] properties,
            MethodInjectionSite[] methods,
            InjectableInterfaceSite[] injectableInterfaces)
        {
            Constructor = constructor;
            ConstructorParameters = constructorParameters;
            Fields = fields;
            Properties = properties;
            Methods = methods;
            InjectableInterfaces = injectableInterfaces;
        }
    }
}

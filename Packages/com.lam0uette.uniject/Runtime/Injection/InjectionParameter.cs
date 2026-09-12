using System;

namespace LaM0uette.UniJect
{
    public readonly struct InjectionParameter
    {
        public ServiceIdentifier Identifier { get; }
        public bool Optional { get; }
        public object DefaultValue { get; }
        public string Name { get; }

        public Type ContractType
        {
            get { return Identifier.ContractType; }
        }

        public InjectionParameter(ServiceIdentifier identifier, bool optional, object defaultValue, string name)
        {
            Identifier = identifier;
            Optional = optional;
            DefaultValue = defaultValue;
            Name = name;
        }
    }
}

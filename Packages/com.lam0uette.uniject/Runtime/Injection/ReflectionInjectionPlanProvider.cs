using System;
using System.Collections.Generic;
using System.Reflection;

namespace LaM0uette.UniJect
{
    public sealed class ReflectionInjectionPlanProvider : IInjectionPlanProvider
    {
        #region Statements

        private static readonly Dictionary<Type, InjectionPlan> PLANS =
            new Dictionary<Type, InjectionPlan>(TypeReferenceComparer.Instance);

        private static readonly Type[] INJECTABLE_DEFINITIONS = InjectableDefinitions.ALL;

        private static readonly string[] STOP_TYPE_NAMES =
        {
            "UnityEngine.MonoBehaviour",
            "UnityEngine.ScriptableObject"
        };

        private const BindingFlags LEVEL_FLAGS =
            BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic;

        public static readonly ReflectionInjectionPlanProvider Instance = new ReflectionInjectionPlanProvider();

        #endregion

        #region Methods

        public InjectionPlan GetPlan(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            lock (PLANS)
            {
                if (PLANS.TryGetValue(type, out InjectionPlan cached))
                    return cached;
            }

            InjectionPlan plan = BuildPlan(type);

            lock (PLANS)
            {
                PLANS[type] = plan;
            }

            return plan;
        }


        private static InjectionPlan BuildPlan(Type type)
        {
            ConstructorInfo constructor = ConstructorSelector.Select(type);
            InjectionParameter[] constructorParameters = constructor == null
                ? new InjectionParameter[0]
                : ReadParameters(constructor.GetParameters());

            List<Type> levels = new List<Type>();

            for (Type current = type; current != null && !IsStopType(current); current = current.BaseType)
                levels.Add(current);

            levels.Reverse();

            List<FieldInjectionSite> fields = new List<FieldInjectionSite>();
            List<PropertyInjectionSite> properties = new List<PropertyInjectionSite>();
            List<MethodInjectionSite> methods = new List<MethodInjectionSite>();

            for (int i = 0; i < levels.Count; i++)
                CollectLevel(levels[i], fields, properties, methods);

            InjectableInterfaceSite[] injectableInterfaces = CollectInjectableInterfaces(type, methods);

            if (constructor == null && fields.Count == 0 && properties.Count == 0 && methods.Count == 0 &&
                injectableInterfaces.Length == 0)
            {
                return InjectionPlan.EMPTY;
            }

            return new InjectionPlan(
                constructor,
                constructorParameters,
                fields.ToArray(),
                properties.ToArray(),
                methods.ToArray(),
                injectableInterfaces);
        }

        private static void CollectLevel(
            Type level,
            List<FieldInjectionSite> fields,
            List<PropertyInjectionSite> properties,
            List<MethodInjectionSite> methods)
        {
            FieldInfo[] levelFields = level.GetFields(LEVEL_FLAGS);

            for (int i = 0; i < levelFields.Length; i++)
            {
                FieldInfo field = levelFields[i];
                InjectAttributeBase attribute = GetAttribute(field);

                if (attribute == null)
                    continue;

                if (field.IsStatic)
                    throw new InvalidInjectionTargetException(level, field.Name, "the field is static");

                if (field.IsInitOnly)
                    throw new InvalidInjectionTargetException(level, field.Name, "the field is readonly");

                fields.Add(new FieldInjectionSite(field, ToParameter(attribute, field.FieldType, field.Name)));
            }

            PropertyInfo[] levelProperties = level.GetProperties(LEVEL_FLAGS);

            for (int i = 0; i < levelProperties.Length; i++)
            {
                PropertyInfo property = levelProperties[i];
                InjectAttributeBase attribute = GetAttribute(property);

                if (attribute == null)
                    continue;

                if (property.GetIndexParameters().Length > 0)
                    throw new InvalidInjectionTargetException(level, property.Name, "the property is an indexer");

                MethodInfo setter = property.GetSetMethod(true);

                if (setter == null)
                    throw new InvalidInjectionTargetException(level, property.Name, "the property has no setter");

                if (setter.IsStatic)
                    throw new InvalidInjectionTargetException(level, property.Name, "the property is static");

                properties.Add(new PropertyInjectionSite(
                    setter,
                    ToParameter(attribute, property.PropertyType, property.Name)));
            }

            MethodInfo[] levelMethods = level.GetMethods(LEVEL_FLAGS);

            for (int i = 0; i < levelMethods.Length; i++)
            {
                MethodInfo method = levelMethods[i];
                InjectAttributeBase attribute = GetAttribute(method);

                if (attribute == null)
                    continue;

                if (method.IsStatic)
                    throw new InvalidInjectionTargetException(level, method.Name, "the method is static");

                methods.Add(new MethodInjectionSite(method, ReadParameters(method.GetParameters())));
            }
        }

        private static InjectableInterfaceSite[] CollectInjectableInterfaces(
            Type type,
            List<MethodInjectionSite> methods)
        {
            if (INJECTABLE_DEFINITIONS.Length == 0)
                return new InjectableInterfaceSite[0];

            Type[] interfaces = type.GetInterfaces();
            List<InjectableInterfaceSite> sites = new List<InjectableInterfaceSite>();

            for (int i = 0; i < interfaces.Length; i++)
            {
                Type candidate = interfaces[i];

                if (!candidate.IsGenericType || !IsInjectableDefinition(candidate.GetGenericTypeDefinition()))
                    continue;

                InterfaceMapping mapping = type.GetInterfaceMap(candidate);

                for (int j = 0; j < mapping.InterfaceMethods.Length; j++)
                {
                    MethodInfo target = mapping.TargetMethods[j];
                    RemoveMethodSite(methods, target);
                    sites.Add(new InjectableInterfaceSite(target, ReadParameters(target.GetParameters())));
                }
            }

            return sites.ToArray();
        }

        private static bool IsInjectableDefinition(Type definition)
        {
            for (int i = 0; i < INJECTABLE_DEFINITIONS.Length; i++)
            {
                if (ReferenceEquals(INJECTABLE_DEFINITIONS[i], definition))
                    return true;
            }

            return false;
        }

        private static void RemoveMethodSite(List<MethodInjectionSite> methods, MethodInfo target)
        {
            for (int i = methods.Count - 1; i >= 0; i--)
            {
                if (methods[i].Method == target)
                    methods.RemoveAt(i);
            }
        }

        private static InjectionParameter[] ReadParameters(ParameterInfo[] parameters)
        {
            if (parameters.Length == 0)
                return new InjectionParameter[0];

            InjectionParameter[] result = new InjectionParameter[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                InjectAttributeBase attribute = GetAttribute(parameter);
                ReadAttribute(attribute, out object id, out bool optional);

                object defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;
                bool hasDefault = parameter.HasDefaultValue;

                result[i] = new InjectionParameter(
                    new ServiceIdentifier(parameter.ParameterType, id),
                    optional || hasDefault,
                    defaultValue,
                    parameter.Name);
            }

            return result;
        }

        private static InjectionParameter ToParameter(InjectAttributeBase attribute, Type contractType, string name)
        {
            ReadAttribute(attribute, out object id, out bool optional);
            return new InjectionParameter(new ServiceIdentifier(contractType, id), optional, null, name);
        }

        private static void ReadAttribute(InjectAttributeBase attribute, out object id, out bool optional)
        {
            if (attribute is InjectOptionalAttribute optionalAttribute)
            {
                id = optionalAttribute.Id;
                optional = true;
                return;
            }

            id = attribute is InjectAttribute injectAttribute ? injectAttribute.Id : null;
            optional = false;
        }

        private static InjectAttributeBase GetAttribute(MemberInfo member)
        {
            object[] attributes = member.GetCustomAttributes(typeof(InjectAttributeBase), false);
            return attributes.Length == 0 ? null : (InjectAttributeBase)attributes[0];
        }

        private static InjectAttributeBase GetAttribute(ParameterInfo parameter)
        {
            object[] attributes = parameter.GetCustomAttributes(typeof(InjectAttributeBase), false);
            return attributes.Length == 0 ? null : (InjectAttributeBase)attributes[0];
        }

        private static bool IsStopType(Type type)
        {
            if (type == typeof(object))
                return true;

            for (int i = 0; i < STOP_TYPE_NAMES.Length; i++)
            {
                if (type.FullName == STOP_TYPE_NAMES[i])
                    return true;
            }

            return false;
        }

        #endregion
    }
}

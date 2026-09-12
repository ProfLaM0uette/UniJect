using System;
using System.Collections.Generic;
using System.Reflection;

namespace LaM0uette.UniJect
{
    public sealed class ReflectionInjector : IInjector
    {
        #region Statements

        private static readonly object[] NO_ARGUMENTS = new object[0];

        private readonly IInjectionPlanProvider _plans;

        public ReflectionInjector()
            : this(ReflectionInjectionPlanProvider.Instance)
        {
        }

        public ReflectionInjector(IInjectionPlanProvider plans)
        {
            _plans = plans ?? throw new ArgumentNullException(nameof(plans));
        }

        #endregion

        #region Methods

        public InjectionPlan GetPlan(Type type)
        {
            return _plans.GetPlan(type);
        }

        public object CreateInstance(Type concreteType, in ResolutionContext context)
        {
            if (concreteType == null)
                throw new ArgumentNullException(nameof(concreteType));

            InjectionPlan plan = _plans.GetPlan(concreteType);

            if (plan.Constructor == null)
                throw new NoSuitableConstructorException(concreteType);

            object[] arguments = BuildConstructorArguments(plan, concreteType, in context);

            try
            {
                return plan.Constructor.Invoke(arguments);
            }
            catch (TargetInvocationException exception)
            {
                throw new ActivationException(concreteType, context.Path, exception.InnerException ?? exception);
            }
        }

        public void Inject(object target, in ResolutionContext context)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            Type targetType = target.GetType();
            InjectionPlan plan = _plans.GetPlan(targetType);

            if (!plan.HasMemberSites)
                return;

            InjectFields(target, targetType, plan, in context);
            InjectProperties(target, targetType, plan, in context);
            InjectMethods(target, targetType, plan, in context);
            InjectInterfaces(target, targetType, plan, in context);
        }


        private object[] BuildConstructorArguments(
            InjectionPlan plan,
            Type concreteType,
            in ResolutionContext context)
        {
            InjectionParameter[] parameters = plan.ConstructorParameters;

            if (parameters.Length == 0)
                return NO_ARGUMENTS;

            object[] arguments = new object[parameters.Length];
            List<object> explicitArguments = CopyArguments(context.Arguments);

            for (int i = 0; i < parameters.Length; i++)
            {
                if (TryTakeExplicit(explicitArguments, parameters[i].ContractType, out object supplied))
                {
                    arguments[i] = supplied;
                    continue;
                }

                arguments[i] = ResolveParameter(
                    parameters[i],
                    concreteType,
                    null,
                    InjectionSiteKind.Constructor,
                    in context);
            }

            return arguments;
        }

        private void InjectFields(object target, Type targetType, InjectionPlan plan, in ResolutionContext context)
        {
            for (int i = 0; i < plan.Fields.Length; i++)
            {
                FieldInjectionSite site = plan.Fields[i];

                if (TryResolveMember(site.Parameter, targetType, target, InjectionSiteKind.Field, in context,
                        out object value))
                {
                    site.Field.SetValue(target, value);
                }
            }
        }

        private void InjectProperties(object target, Type targetType, InjectionPlan plan, in ResolutionContext context)
        {
            for (int i = 0; i < plan.Properties.Length; i++)
            {
                PropertyInjectionSite site = plan.Properties[i];

                if (!TryResolveMember(site.Parameter, targetType, target, InjectionSiteKind.Property, in context,
                        out object value))
                {
                    continue;
                }

                object[] arguments = { value };
                Invoke(site.Setter, target, arguments, targetType, context.Path);
            }
        }

        private void InjectMethods(object target, Type targetType, InjectionPlan plan, in ResolutionContext context)
        {
            for (int i = 0; i < plan.Methods.Length; i++)
            {
                MethodInjectionSite site = plan.Methods[i];
                object[] arguments = BuildMemberArguments(
                    site.Parameters,
                    targetType,
                    target,
                    InjectionSiteKind.Method,
                    in context);

                Invoke(site.Method, target, arguments, targetType, context.Path);
            }
        }

        private void InjectInterfaces(object target, Type targetType, InjectionPlan plan, in ResolutionContext context)
        {
            for (int i = 0; i < plan.InjectableInterfaces.Length; i++)
            {
                InjectableInterfaceSite site = plan.InjectableInterfaces[i];
                object[] arguments = BuildMemberArguments(
                    site.Parameters,
                    targetType,
                    target,
                    InjectionSiteKind.InjectableInterface,
                    in context);

                Invoke(site.Method, target, arguments, targetType, context.Path);
            }
        }

        private object[] BuildMemberArguments(
            InjectionParameter[] parameters,
            Type targetType,
            object target,
            InjectionSiteKind siteKind,
            in ResolutionContext context)
        {
            if (parameters.Length == 0)
                return NO_ARGUMENTS;

            object[] arguments = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                arguments[i] = ResolveParameter(parameters[i], targetType, target, siteKind, in context);

            return arguments;
        }

        private object ResolveParameter(
            in InjectionParameter parameter,
            Type consumerType,
            object consumerInstance,
            InjectionSiteKind siteKind,
            in ResolutionContext context)
        {
            ResolutionRequest request = new ResolutionRequest(
                parameter.Identifier,
                consumerType,
                consumerInstance,
                parameter.Name,
                siteKind);

            if (!parameter.Optional)
                return context.ResolveDependency(in request);

            if (context.TryResolveDependency(in request, out object instance))
                return instance;

            return parameter.DefaultValue ?? DefaultOf(parameter.ContractType);
        }

        private bool TryResolveMember(
            in InjectionParameter parameter,
            Type consumerType,
            object consumerInstance,
            InjectionSiteKind siteKind,
            in ResolutionContext context,
            out object value)
        {
            ResolutionRequest request = new ResolutionRequest(
                parameter.Identifier,
                consumerType,
                consumerInstance,
                parameter.Name,
                siteKind);

            if (!parameter.Optional)
            {
                value = context.ResolveDependency(in request);
                return true;
            }

            return context.TryResolveDependency(in request, out value);
        }

        private static void Invoke(MethodInfo method, object target, object[] arguments, Type targetType,
            ResolutionPath path)
        {
            try
            {
                method.Invoke(target, arguments);
            }
            catch (TargetInvocationException exception)
            {
                throw new ActivationException(targetType, path, exception.InnerException ?? exception);
            }
        }

        private static List<object> CopyArguments(IReadOnlyList<object> arguments)
        {
            if (arguments == null || arguments.Count == 0)
                return null;

            List<object> copy = new List<object>(arguments.Count);

            for (int i = 0; i < arguments.Count; i++)
                copy.Add(arguments[i]);

            return copy;
        }

        private static bool TryTakeExplicit(List<object> arguments, Type contractType, out object value)
        {
            value = null;

            if (arguments == null || arguments.Count == 0)
                return false;

            for (int i = 0; i < arguments.Count; i++)
            {
                object candidate = arguments[i];

                if (candidate == null || !contractType.IsInstanceOfType(candidate))
                    continue;

                value = candidate;
                arguments.RemoveAt(i);
                return true;
            }

            return false;
        }

        private static object DefaultOf(Type type)
        {
            return type.IsValueType ? System.Activator.CreateInstance(type) : null;
        }

        #endregion
    }
}

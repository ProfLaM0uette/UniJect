using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LaM0uette.UniJect
{
    internal static class ResolutionMessage
    {
        #region Statements

        private const string PREFIX = "UniJect: ";

        #endregion

        #region Methods

        public static string BindingNotFound(
            Type contractType,
            object id,
            ResolutionPath path,
            IReadOnlyList<SelectionFailure> nearMatches,
            string container = null,
            string installers = null)
        {
            string contract = Describe(contractType, id);
            StringBuilder builder = new StringBuilder();

            builder.Append(PREFIX).Append("resolving ").Append(DescribeChain(path)).Append(": no binding for ")
                .Append(contract).Append('.');

            AppendChain(builder, path);
            builder.AppendLine().AppendLine().Append("Near matches for ").Append(contract).Append(':');

            if (nearMatches == null || nearMatches.Count == 0)
            {
                builder.AppendLine().Append("  (none)");
            }
            else
            {
                for (int i = 0; i < nearMatches.Count; i++)
                    builder.AppendLine().Append("  ").Append(Pad(nearMatches[i].Registration)).Append(nearMatches[i]);
            }

            AppendContext(builder, container, installers);

            builder.AppendLine().AppendLine().Append("Fix: bind ").Append(contract)
                .Append(" in one of your installers, mark the injection site [InjectOptional], ")
                .Append("or use TryResolve if it is optional.");

            return builder.ToString();
        }

        public static string AmbiguousBinding(
            Type contractType,
            object id,
            ResolutionPath path,
            IReadOnlyList<Registration> candidates,
            string container = null,
            string installers = null)
        {
            string contract = Describe(contractType, id);
            StringBuilder builder = new StringBuilder();

            builder.Append(PREFIX).Append("resolving ").Append(DescribeChain(path)).Append(": ")
                .Append(candidates == null ? 0 : candidates.Count).Append(" bindings match ").Append(contract)
                .Append(" and none is more specific.");

            AppendChain(builder, path);
            builder.AppendLine().AppendLine().Append("Candidates:");

            if (candidates != null)
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    Registration candidate = candidates[i];
                    string condition = candidate.Condition == null ? "no condition" : candidate.Condition.Describe();
                    builder.AppendLine().Append("  ").Append(Pad(candidate)).Append(candidate.Origin).Append(" — ")
                        .Append(condition);
                }
            }

            AppendContext(builder, container, installers);

            builder.AppendLine().AppendLine()
                .Append("Fix: give one of them .WithId(...) and the injection site [Inject(...)], ")
                .Append("or narrow one with .WhenInjectedInto<T>().");

            return builder.ToString();
        }

        public static string DuplicateBinding(
            Type contractType,
            object id,
            Type concreteType,
            BindingOrigin first,
            BindingOrigin second)
        {
            return PREFIX + Describe(contractType, id) + " is already bound to " + Name(concreteType) + " at " +
                   first + ". Duplicate at " + second +
                   ". Use .WithId(...) to distinguish them, or .IfNotBound() if this is a default.";
        }

        public static string CircularDependency(ResolutionPath path)
        {
            StringBuilder builder = new StringBuilder();
            ServiceIdentifier head = path != null && path.Count > 0 ? path.Frames[0].Identifier : default;

            builder.Append(PREFIX).Append("circular dependency while resolving ").Append(head).Append('.');
            builder.AppendLine().AppendLine().Append("Path: ").Append(path == null ? "<unknown>" : path.Describe());
            builder.AppendLine().AppendLine()
                .Append("Fix: break the cycle with an IFactory<T>, or move one dependency from the constructor ")
                .Append("to an [Inject] field on a singleton binding.");

            return builder.ToString();
        }

        public static string ResolutionDepthExceeded(int depth, Type contractType, ResolutionPath path)
        {
            return PREFIX + "resolution exceeded " + depth + " levels while resolving " + Name(contractType) +
                   ". Path: " + (path == null ? "<unknown>" : path.Describe()) +
                   ". This is almost always a cycle through FromMethod or CreateInstance, " +
                   "which the build-time check cannot see.";
        }

        public static string AmbiguousConstructor(Type concreteType, IReadOnlyList<ConstructorInfo> candidates)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(PREFIX).Append(Name(concreteType)).Append(" has ")
                .Append(candidates == null ? 0 : candidates.Count)
                .Append(" candidate constructors and no way to choose. Candidates: ");

            if (candidates != null)
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    if (i > 0)
                        builder.Append(", ");

                    builder.Append(DescribeConstructor(candidates[i]));
                }
            }

            builder.Append(". Mark exactly one with [Inject].");
            return builder.ToString();
        }

        public static string NoSuitableConstructor(Type concreteType)
        {
            return PREFIX + Name(concreteType) + " has no constructor the container can call. " +
                   "Add a constructor, or bind it with .FromInstance(...) / .FromMethod(...).";
        }

        public static string InvalidBinding(string code, Type contractType, BindingOrigin origin, string reason)
        {
            return PREFIX + code + " — " + reason + ". Binding for " + Name(contractType) + " at " + origin + ".";
        }

        public static string InvalidInjectionTarget(Type declaringType, string memberName, string reason)
        {
            return PREFIX + "[Inject] on " + Name(declaringType) + "." + memberName + " is invalid — " + reason +
                   ". Remove the attribute or change the member.";
        }

        public static string Activation(Type concreteType, ResolutionPath path, bool returnedNull)
        {
            string tail = returnedNull ? " returned null." : " failed. See the inner exception.";
            return PREFIX + "activating " + Name(concreteType) + tail + " Path: " +
                   (path == null ? "<unknown>" : path.Describe());
        }

        public static string ContainerPhase(BuildPhase actual, BuildPhase required, string operation)
        {
            return PREFIX + "cannot " + operation + " while the container is " + actual + "; it must be " + required +
                   ". Bindings are declared in Install() and resolved after Build().";
        }


        private static void AppendContext(StringBuilder builder, string container, string installers)
        {
            if (container != null)
                builder.AppendLine().AppendLine().Append("Container: ").Append(container);

            if (installers != null)
                builder.AppendLine().Append("Installers: ").Append(installers);
        }

        private static void AppendChain(StringBuilder builder, ResolutionPath path)
        {
            builder.AppendLine().AppendLine().Append("Resolution chain:").AppendLine();
            builder.Append(path == null ? "  <root>" : path.DescribeIndented());
        }

        private static string DescribeChain(ResolutionPath path)
        {
            return path == null || path.Count == 0 ? "<root>" : path.Describe();
        }

        private static string Describe(Type contractType, object id)
        {
            return id == null ? Name(contractType) : Name(contractType) + " (id: " + id + ")";
        }

        private static string Name(Type type)
        {
            return type == null ? "<unknown>" : type.Name;
        }

        private static string Pad(Registration registration)
        {
            string label = registration == null ? "<unknown>" : registration.ToString();
            return label.Length >= 32 ? label + " " : label.PadRight(32);
        }

        private static string DescribeConstructor(ConstructorInfo constructor)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            StringBuilder builder = new StringBuilder();

            builder.Append('(');

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i > 0)
                    builder.Append(", ");

                builder.Append(parameters[i].ParameterType.Name);
            }

            builder.Append(')');
            return builder.ToString();
        }

        #endregion
    }
}

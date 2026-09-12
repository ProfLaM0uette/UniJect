using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal static class CaptiveDependencyValidator
    {
        public static void Validate(Registration registration, CallSite site, List<ValidationIssue> issues)
        {
            if (registration.Lifetime != Lifetime.Singleton)
                return;

            HashSet<CallSite> visited = new HashSet<CallSite>();
            Walk(registration, site, visited, issues);
        }


        private static void Walk(
            Registration registration,
            CallSite site,
            HashSet<CallSite> visited,
            List<ValidationIssue> issues)
        {
            if (site.Dependencies == null || !visited.Add(site))
                return;

            for (int i = 0; i < site.Dependencies.Length; i++)
            {
                CallSite dependency = site.Dependencies[i];

                if (dependency.Lifetime == Lifetime.Scoped)
                {
                    issues.Add(new ValidationIssue(
                        IssueCode.UJ012,
                        ValidationSeverity.Error,
                        "UniJect: " + IssueCode.UJ012 + " — captive dependency: the singleton " +
                        Describe(registration.ConcreteType) + " depends on " +
                        Describe(dependency.Registration.ConcreteType) +
                        ", which is scoped. The scoped instance would be captured for the lifetime of the " +
                        "singleton and outlive its own scope. Make the dependency a singleton, make the " +
                        "consumer scoped, or inject a factory.",
                        registration.ContractTypes[0],
                        registration.Origin));

                    continue;
                }

                Walk(registration, dependency, visited, issues);
            }
        }

        private static string Describe(System.Type type)
        {
            return type == null ? "<unknown>" : type.Name;
        }
    }
}

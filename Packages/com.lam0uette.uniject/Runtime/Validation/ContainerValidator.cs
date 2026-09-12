using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal static class ContainerValidator
    {
        public static ValidationReport Validate(DIContainer container)
        {
            List<ValidationIssue> issues = new List<ValidationIssue>();
            IReadOnlyList<Registration> registrations = container.Registrations;

            for (int i = 0; i < registrations.Count; i++)
                ValidateRegistration(container, registrations[i], issues);

            return new ValidationReport(issues);
        }


        private static void ValidateRegistration(
            DIContainer container,
            Registration registration,
            List<ValidationIssue> issues)
        {
            ResolutionRequest request = new ResolutionRequest(
                new ServiceIdentifier(registration.ContractTypes[0], registration.Id),
                null,
                null,
                null,
                InjectionSiteKind.Constructor);

            CallSiteChain chain = new CallSiteChain();
            CallSite site = new CallSite(registration, true);

            chain.Push(in request, registration.Lifetime);

            try
            {
                container.CallSites.BuildDependencies(site, chain);
            }
            catch (InvalidBindingException exception)
            {
                issues.Add(Issue(exception.Code, registration, exception.Message));
            }
            catch (UniJectException exception)
            {
                issues.Add(Issue(IssueCode.UJ001, registration, exception.Message));
            }
            finally
            {
                chain.Pop();
            }
        }

        private static ValidationIssue Issue(string code, Registration registration, string message)
        {
            return new ValidationIssue(
                code,
                ValidationSeverity.Error,
                message,
                registration.ContractTypes[0],
                registration.Origin);
        }
    }
}

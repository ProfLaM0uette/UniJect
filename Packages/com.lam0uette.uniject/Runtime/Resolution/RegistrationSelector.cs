using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal static class RegistrationSelector
    {
        public static Registration Select(
            RegistrationGroup group,
            in ResolutionRequest request,
            List<SelectionFailure> rejected,
            out List<Registration> ambiguous)
        {
            ambiguous = null;

            Registration conditional = null;
            Registration unconditional = null;
            int conditionalCount = 0;
            int unconditionalCount = 0;

            IReadOnlyList<Registration> registrations = group.Registrations;

            for (int i = 0; i < registrations.Count; i++)
            {
                Registration registration = registrations[i];

                if (registration.Condition == null)
                {
                    unconditionalCount++;
                    unconditional = unconditional ?? registration;
                    continue;
                }

                if (!registration.Condition.Matches(in request))
                {
                    rejected?.Add(new SelectionFailure(registration, registration.Condition.Describe()));
                    continue;
                }

                conditionalCount++;
                conditional = conditional ?? registration;
            }

            if (conditionalCount == 1)
                return conditional;

            if (conditionalCount > 1)
            {
                ambiguous = CollectMatching(registrations, in request, true);
                return null;
            }

            if (unconditionalCount == 1)
                return unconditional;

            if (unconditionalCount > 1)
            {
                ambiguous = CollectMatching(registrations, in request, false);
                return null;
            }

            return null;
        }

        private static List<Registration> CollectMatching(
            IReadOnlyList<Registration> registrations,
            in ResolutionRequest request,
            bool conditional)
        {
            List<Registration> matching = new List<Registration>();

            for (int i = 0; i < registrations.Count; i++)
            {
                Registration registration = registrations[i];

                if (conditional)
                {
                    if (registration.Condition != null && registration.Condition.Matches(in request))
                        matching.Add(registration);

                    continue;
                }

                if (registration.Condition == null)
                    matching.Add(registration);
            }

            return matching;
        }
    }
}

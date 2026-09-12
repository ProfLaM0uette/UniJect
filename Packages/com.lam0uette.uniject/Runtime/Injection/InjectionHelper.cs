using System;

namespace LaM0uette.UniJect
{
    public static class InjectionHelper
    {
        public static void InjectAll(IInstantiator instantiator, object target)
        {
            if (instantiator == null)
                throw new ArgumentNullException(nameof(instantiator));

            if (target == null)
                return;

            instantiator.Inject(target);
        }
    }
}

using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal sealed class RegistrationGroup
    {
        #region Statements

        private readonly List<Registration> _registrations = new List<Registration>();

        public IReadOnlyList<Registration> Registrations
        {
            get { return _registrations; }
        }

        public bool AllConditionsStatic { get; private set; } = true;

        #endregion

        #region Methods

        public void Add(Registration registration)
        {
            _registrations.Add(registration);

            if (registration.Condition != null && !(registration.Condition is IStaticCondition))
                AllConditionsStatic = false;
        }

        #endregion
    }
}

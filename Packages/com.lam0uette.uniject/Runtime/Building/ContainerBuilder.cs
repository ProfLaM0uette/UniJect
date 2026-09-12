using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class ContainerBuilder : IContainerBuilder
    {
        #region Statements

        private readonly List<BindingDraft> _drafts = new List<BindingDraft>();
        private readonly List<Registration> _registrations = new List<Registration>();
        private readonly List<Action<DIContainer>> _callbacks = new List<Action<DIContainer>>();

        public BuildPhase Phase { get; internal set; }

        internal DIContainer ParentContainer { get; }

        internal IReadOnlyList<BindingDraft> Drafts
        {
            get { return _drafts; }
        }

        internal IReadOnlyList<Registration> Registrations
        {
            get { return _registrations; }
        }

        internal IReadOnlyList<Action<DIContainer>> Callbacks
        {
            get { return _callbacks; }
        }

        public ContainerBuilder()
            : this(null)
        {
        }

        internal ContainerBuilder(DIContainer parentContainer)
        {
            ParentContainer = parentContainer;
            Phase = BuildPhase.Declaring;
        }

        #endregion

        #region Methods

        public void Add(Registration registration)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            AssertDeclaring("add a binding");
            _registrations.Add(registration);
        }

        public bool TryAdd(Registration registration)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            AssertDeclaring("add a binding");

            for (int i = 0; i < registration.ContractTypes.Count; i++)
            {
                if (IsDeclared(registration.ContractTypes[i], registration.Id))
                    return false;
            }

            _registrations.Add(registration);
            return true;
        }

        public void OnBuilt(Action<DIContainer> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            AssertDeclaring("register an OnBuilt callback");
            _callbacks.Add(callback);
        }

        public DIContainer Build(ContainerOptions options)
        {
            return BuildPipeline.Build(this, options ?? ContainerOptions.Default);
        }


        internal BindingDraft AddDraft(BindingDraft draft)
        {
            AssertDeclaring("declare a binding");
            _drafts.Add(draft);
            return draft;
        }

        internal bool IsDeclared(Type contractType, object id)
        {
            for (int i = 0; i < _drafts.Count; i++)
            {
                BindingDraft draft = _drafts[i];

                if (!Equals(draft.Id, id))
                    continue;

                for (int j = 0; j < draft.ContractTypes.Count; j++)
                {
                    if (ReferenceEquals(draft.ContractTypes[j], contractType))
                        return true;
                }
            }

            for (int i = 0; i < _registrations.Count; i++)
            {
                Registration registration = _registrations[i];

                if (!Equals(registration.Id, id))
                    continue;

                for (int j = 0; j < registration.ContractTypes.Count; j++)
                {
                    if (ReferenceEquals(registration.ContractTypes[j], contractType))
                        return true;
                }
            }

            return false;
        }

        private void AssertDeclaring(string operation)
        {
            if (Phase != BuildPhase.Declaring)
                throw new ContainerPhaseException(Phase, BuildPhase.Declaring, operation);
        }

        #endregion
    }
}

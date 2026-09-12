using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal static class BuildPipeline
    {
        public static DIContainer Build(ContainerBuilder builder, ContainerOptions options)
        {
            if (builder.Phase != BuildPhase.Declaring)
                throw new ContainerPhaseException(builder.Phase, BuildPhase.Declaring, "build the container");

            builder.Phase = BuildPhase.Building;

            IInjector injector = options.ResolveInjector();
            List<Registration> registrations = new List<Registration>(builder.Registrations);

            IReadOnlyList<BindingDraft> drafts = builder.Drafts;

            for (int i = 0; i < drafts.Count; i++)
            {
                Registration registration = ToRegistration(drafts[i], injector, registrations, options);

                if (registration != null)
                    registrations.Add(registration);
            }

            Registration[] frozen = registrations.ToArray();
            Dictionary<ServiceIdentifier, RegistrationGroup> index = Index(frozen);

            int slotOffset = builder.ParentContainer == null ? 0 : builder.ParentContainer.SlotCount;

            for (int i = 0; i < frozen.Length; i++)
                frozen[i].StoreSlot = slotOffset + i;

            DIContainer container = new DIContainer(
                builder.ParentContainer,
                frozen,
                index,
                slotOffset + frozen.Length,
                options);

            AddSelfRegistrations(index, container, injector);
            WarmInjectionPlans(frozen, injector);

            if (options.ValidateOnBuild)
            {
                ValidationReport report = ContainerValidator.Validate(container);

                if (report.ErrorCount > 0)
                    throw new ContainerValidationException(report);
            }

            builder.Phase = BuildPhase.Built;

            IReadOnlyList<Action<DIContainer>> callbacks = builder.Callbacks;

            for (int i = 0; i < callbacks.Count; i++)
                callbacks[i](container);

            return container;
        }


        private static Registration ToRegistration(
            BindingDraft draft,
            IInjector injector,
            List<Registration> alreadyEmitted,
            ContainerOptions options)
        {
            if (draft.ContractTypes.Count == 0)
                throw new InvalidBindingException(IssueCode.UJ003, null, draft.Origin, "the binding declares no contract");

            if (draft.IfNotBound && IsBound(alreadyEmitted, draft))
                return null;

            IActivatorSource source = ResolveSource(draft, options);

            Lifetime lifetime = ResolveLifetime(draft, source);
            ValidateCombination(draft, source, lifetime);

            IActivator activator = source.Build(draft.ConcreteType, injector);

            IBindingCondition condition = draft.Conditions.Count == 0
                ? null
                : CompositeCondition.Create(draft.Conditions.ToArray());

            return new Registration(
                draft.ContractTypes.ToArray(),
                draft.ConcreteType,
                lifetime,
                draft.Id,
                activator,
                condition,
                draft.NonLazy,
                draft.Origin);
        }

        private static IActivatorSource ResolveSource(BindingDraft draft, ContainerOptions options)
        {
            IActivatorSource source = draft.Source;

            if (source != null && !(source is ConstructorActivatorSource))
                return source;

            IActivatorSource resolved = options.DefaultSourceRule?.Resolve(draft.ConcreteType);

            if (resolved != null)
                return resolved;

            if (source != null)
                return source;

            if (draft.ConcreteType == null || draft.ConcreteType.IsAbstract || draft.ConcreteType.IsInterface)
            {
                throw new InvalidBindingException(
                    IssueCode.UJ003,
                    draft.ContractTypes[0],
                    draft.Origin,
                    "the concrete type is abstract or an interface and no From* source was declared");
            }

            return ConstructorActivatorSource.Instance;
        }

        private static Lifetime ResolveLifetime(BindingDraft draft, IActivatorSource source)
        {
            if (draft.LifetimeExplicit)
                return draft.Lifetime;

            return source is InstanceActivatorSource ? Lifetime.Singleton : draft.Lifetime;
        }

        private static void ValidateCombination(BindingDraft draft, IActivatorSource source, Lifetime lifetime)
        {
            Type contract = draft.ContractTypes[0];

            if (lifetime == Lifetime.Transient && draft.NonLazy)
            {
                throw new InvalidBindingException(IssueCode.UJ005, contract, draft.Origin,
                    ".AsTransient() cannot be combined with .NonLazy(): there is no single instance to create eagerly");
            }

            if (lifetime == Lifetime.Transient && source is InstanceActivatorSource)
            {
                throw new InvalidBindingException(IssueCode.UJ006, contract, draft.Origin,
                    ".FromInstance(...) cannot be combined with .AsTransient(): one instance cannot be transient");
            }

            if (!(source is AliasActivatorSource alias))
                return;

            for (int i = 0; i < draft.ContractTypes.Count; i++)
            {
                if (ReferenceEquals(draft.ContractTypes[i], draft.ConcreteType) && Equals(alias.Id, draft.Id))
                {
                    throw new InvalidBindingException(IssueCode.UJ007, contract, draft.Origin,
                        ".FromResolve() points at its own (contract, id) and would resolve to itself forever");
                }
            }
        }

        private static bool IsBound(List<Registration> registrations, BindingDraft draft)
        {
            for (int i = 0; i < registrations.Count; i++)
            {
                Registration registration = registrations[i];

                if (!Equals(registration.Id, draft.Id))
                    continue;

                for (int j = 0; j < registration.ContractTypes.Count; j++)
                {
                    for (int k = 0; k < draft.ContractTypes.Count; k++)
                    {
                        if (ReferenceEquals(registration.ContractTypes[j], draft.ContractTypes[k]))
                            return true;
                    }
                }
            }

            return false;
        }

        private static Dictionary<ServiceIdentifier, RegistrationGroup> Index(Registration[] registrations)
        {
            Dictionary<ServiceIdentifier, RegistrationGroup> index =
                new Dictionary<ServiceIdentifier, RegistrationGroup>(registrations.Length);

            for (int i = 0; i < registrations.Length; i++)
            {
                Registration registration = registrations[i];

                for (int j = 0; j < registration.ContractTypes.Count; j++)
                {
                    ServiceIdentifier identifier =
                        new ServiceIdentifier(registration.ContractTypes[j], registration.Id);

                    if (!index.TryGetValue(identifier, out RegistrationGroup group))
                    {
                        group = new RegistrationGroup();
                        index[identifier] = group;
                    }

                    AssertNotDuplicate(group, registration, identifier);
                    group.Add(registration);
                }
            }

            return index;
        }

        private static void AssertNotDuplicate(
            RegistrationGroup group,
            Registration registration,
            in ServiceIdentifier identifier)
        {
            IReadOnlyList<Registration> existing = group.Registrations;

            for (int i = 0; i < existing.Count; i++)
            {
                if (!ReferenceEquals(existing[i].ConcreteType, registration.ConcreteType))
                    continue;

                throw new DuplicateBindingException(
                    identifier.ContractType,
                    identifier.Id,
                    registration.ConcreteType,
                    existing[i].Origin,
                    registration.Origin);
            }
        }

        private static void AddSelfRegistrations(
            Dictionary<ServiceIdentifier, RegistrationGroup> index,
            DIContainer container,
            IInjector injector)
        {
            AddSelf(index, container, injector, typeof(DIContainer));
            AddSelf(index, container, injector, typeof(IResolver));
            AddSelf(index, container, injector, typeof(IInstantiator));
            AddSelf(index, container, injector, typeof(IServiceProvider));
        }

        private static void AddSelf(
            Dictionary<ServiceIdentifier, RegistrationGroup> index,
            DIContainer container,
            IInjector injector,
            Type contractType)
        {
            ServiceIdentifier identifier = new ServiceIdentifier(contractType, null);

            if (index.ContainsKey(identifier))
                return;

            Registration registration = new Registration(
                new[] { contractType },
                typeof(DIContainer),
                Lifetime.Transient,
                null,
                new InstanceActivator(container, typeof(DIContainer), injector),
                null,
                false,
                BindingOrigin.Unknown);

            RegistrationGroup group = new RegistrationGroup();
            group.Add(registration);
            index[identifier] = group;
        }

        private static void WarmInjectionPlans(Registration[] registrations, IInjector injector)
        {
            for (int i = 0; i < registrations.Length; i++)
            {
                Type concreteType = registrations[i].ConcreteType;

                if (concreteType != null)
                    injector.GetPlan(concreteType);
            }
        }
    }
}

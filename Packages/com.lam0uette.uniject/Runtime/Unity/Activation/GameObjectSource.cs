using System;

namespace LaM0uette.UniJect
{
    public sealed class GameObjectSource : IActivatorSource, IBindingValidation
    {
        #region Statements

        public GameObjectPlacementDraft Placement { get; } = new GameObjectPlacementDraft();
        public IGameObjectFactory Factory { get; set; }
        public bool FromPrefab { get; set; }

        #endregion

        #region Methods

        public void Validate(BindingOrigin origin, bool isChildScope, System.Collections.Generic.List<ValidationIssue> issues)
        {
            if (!Placement.DontDestroyOnLoad)
                return;

            if (Placement.ParentMode == ParentMode.Explicit && Placement.Parent != null)
            {
                issues.Add(new ValidationIssue(
                    IssueCode.UJ008,
                    ValidationSeverity.Warning,
                    "UniJect: " + IssueCode.UJ008 +
                    " — .DontDestroyOnLoad() with an explicit .Parent(...) promotes the parent's whole root " +
                    "hierarchy, not just this object. It works, and it is rarely what you meant.",
                    null,
                    origin));
            }

            if (!isChildScope)
                return;

            issues.Add(new ValidationIssue(
                IssueCode.UJ009,
                ValidationSeverity.Warning,
                "UniJect: " + IssueCode.UJ009 +
                " — .DontDestroyOnLoad() is declared on a scene container rather than the project one, " +
                "so a new instance survives every scene load.",
                null,
                origin));
        }

        public IActivator Build(Type concreteType, IInjector injector)
        {
            return new ComponentActivator(
                concreteType,
                Factory ?? new NewGameObjectFactory(),
                Placement.ToPlacement(),
                injector,
                FromPrefab);
        }

        #endregion
    }
}

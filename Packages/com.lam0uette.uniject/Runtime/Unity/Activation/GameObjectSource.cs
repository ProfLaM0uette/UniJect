using System;

namespace LaM0uette.UniJect
{
    public sealed class GameObjectSource : IActivatorSource
    {
        #region Statements

        public GameObjectPlacementDraft Placement { get; } = new GameObjectPlacementDraft();
        public IGameObjectFactory Factory { get; set; }
        public bool FromPrefab { get; set; }

        #endregion

        #region Methods

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

using System;

namespace LaM0uette.UniJect
{
    public sealed class ConstructorActivatorSource : IActivatorSource
    {
        #region Statements

        public static readonly ConstructorActivatorSource Instance = new ConstructorActivatorSource();

        private ConstructorActivatorSource()
        {
        }

        #endregion

        #region Methods

        public IActivator Build(Type concreteType, IInjector injector)
        {
            return new ConstructorActivator(concreteType, injector);
        }

        #endregion
    }
}

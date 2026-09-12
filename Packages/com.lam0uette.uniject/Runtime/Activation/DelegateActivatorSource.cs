using System;

namespace LaM0uette.UniJect
{
    public sealed class DelegateActivatorSource : IActivatorSource
    {
        #region Statements

        private readonly Func<IResolver, object> _factory;

        public DelegateActivatorSource(Func<IResolver, object> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion

        #region Methods

        public IActivator Build(Type concreteType, IInjector injector)
        {
            return new DelegateActivator(_factory, concreteType, injector);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal sealed class ActivatorProductFactory : IProductFactory
    {
        #region Statements

        private readonly DIContainer _container;
        private readonly IActivator _activator;

        public ActivatorProductFactory(DIContainer container, IActivator activator)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _activator = activator ?? throw new ArgumentNullException(nameof(activator));
        }

        #endregion

        #region Methods

        public object Create(IReadOnlyList<object> arguments)
        {
            return _container.ActivateProduct(_activator, arguments);
        }

        #endregion
    }
}

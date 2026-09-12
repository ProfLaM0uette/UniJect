using System;
using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class ExistingComponentSource : IActivatorSource
    {
        #region Statements

        private readonly GameObject _host;
        private readonly bool _includeChildren;
        private readonly bool _includeInactive;

        public ExistingComponentSource(GameObject host, bool includeChildren, bool includeInactive)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _includeChildren = includeChildren;
            _includeInactive = includeInactive;
        }

        #endregion

        #region Methods

        public IActivator Build(Type concreteType, IInjector injector)
        {
            return new ExistingComponentActivator(
                concreteType,
                _host,
                _includeChildren,
                _includeInactive,
                injector);
        }

        #endregion
    }
}

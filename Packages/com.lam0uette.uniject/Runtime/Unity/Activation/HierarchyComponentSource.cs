using System;

namespace LaM0uette.UniJect
{
    public sealed class HierarchyComponentSource : IActivatorSource
    {
        #region Statements

        private readonly bool _includeInactive;

        public HierarchyComponentSource(bool includeInactive)
        {
            _includeInactive = includeInactive;
        }

        #endregion

        #region Methods

        public IActivator Build(Type concreteType, IInjector injector)
        {
            return new HierarchyComponentActivator(concreteType, _includeInactive, injector);
        }

        #endregion
    }
}

using System;

namespace LaM0uette.UniJect
{
    public sealed class InstanceActivatorSource : IActivatorSource
    {
        #region Statements

        private readonly object _instance;

        public object Instance
        {
            get { return _instance; }
        }

        public InstanceActivatorSource(object instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        #endregion

        #region Methods

        public IActivator Build(Type concreteType, IInjector injector)
        {
            return new InstanceActivator(_instance, concreteType, injector);
        }

        #endregion
    }
}

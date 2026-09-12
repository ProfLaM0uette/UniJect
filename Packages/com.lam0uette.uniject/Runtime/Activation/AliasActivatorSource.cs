using System;

namespace LaM0uette.UniJect
{
    public sealed class AliasActivatorSource : IActivatorSource
    {
        #region Statements

        private readonly object _id;

        public object Id
        {
            get { return _id; }
        }

        public AliasActivatorSource(object id)
        {
            _id = id;
        }

        #endregion

        #region Methods

        public IActivator Build(Type concreteType, IInjector injector)
        {
            return new AliasActivator(new ServiceIdentifier(concreteType, _id), concreteType);
        }

        #endregion
    }
}

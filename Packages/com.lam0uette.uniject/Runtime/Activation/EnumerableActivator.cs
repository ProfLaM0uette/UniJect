using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal sealed class EnumerableActivator : IActivator
    {
        #region Statements

        private static readonly InjectionParameter[] NO_DEPENDENCIES = new InjectionParameter[0];

        private readonly Type _contractType;
        private readonly Type _elementType;

        public Type ProducedType
        {
            get { return _contractType; }
        }

        public Ownership Ownership
        {
            get { return Ownership.None; }
        }

        public IReadOnlyList<InjectionParameter> DeclaredDependencies
        {
            get { return NO_DEPENDENCIES; }
        }

        public Type ElementType
        {
            get { return _elementType; }
        }

        public EnumerableActivator(Type contractType, Type elementType)
        {
            _contractType = contractType;
            _elementType = elementType;
        }

        #endregion

        #region Methods

        public object Create(in ResolutionContext context)
        {
            ResolutionRequest request = context.Request;
            IReadOnlyList<object> items = context.Container.ResolveElements(_elementType, in request);

            return CollectionContract.Materialize(_contractType, _elementType, items);
        }

        public void Inject(object instance, in ResolutionContext context)
        {
        }

        #endregion
    }
}

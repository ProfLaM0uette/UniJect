using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal sealed class FactoryActivator : IActivator
    {
        #region Statements

        private static readonly InjectionParameter[] NO_DEPENDENCIES = new InjectionParameter[0];

        private readonly ProductActivatorResolver _product;
        private readonly Func<IProductFactory, object> _adapter;

        public Type ProducedType { get; }

        public Ownership Ownership
        {
            get { return Ownership.Managed; }
        }

        public IReadOnlyList<InjectionParameter> DeclaredDependencies
        {
            get { return NO_DEPENDENCIES; }
        }

        public FactoryActivator(
            Type producedType,
            ProductActivatorResolver product,
            Func<IProductFactory, object> adapter)
        {
            ProducedType = producedType ?? throw new ArgumentNullException(nameof(producedType));
            _product = product ?? throw new ArgumentNullException(nameof(product));
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        #endregion

        #region Methods

        public object Create(in ResolutionContext context)
        {
            IActivator productActivator = _product.Resolve(context.Container.Options);
            IProductFactory productFactory = new ActivatorProductFactory(context.Container, productActivator);

            return _adapter(productFactory);
        }

        public void Inject(object instance, in ResolutionContext context)
        {
        }

        #endregion
    }
}

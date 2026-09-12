using System;

namespace LaM0uette.UniJect
{
    public sealed class FactoryBinder<TProduct>
    {
        #region Statements

        private readonly FactoryActivatorSource _source;

        public BindingDraft Draft { get; }

        public IProductSourceHolder Product
        {
            get { return _source; }
        }

        internal FactoryBinder(BindingDraft draft, FactoryActivatorSource source)
        {
            Draft = draft;
            _source = source;
        }

        #endregion

        #region Methods

        public FactoryBinder<TProduct> AsSingleton()
        {
            Draft.SetLifetime(Lifetime.Singleton);
            return this;
        }

        public FactoryBinder<TProduct> AsTransient()
        {
            Draft.SetLifetime(Lifetime.Transient);
            return this;
        }

        public FactoryBinder<TProduct> NonLazy()
        {
            Draft.NonLazy = true;
            return this;
        }

        public FactoryBinder<TProduct> WithId(object id)
        {
            Draft.Id = id;
            return this;
        }

        public FactoryBinder<TProduct> FromNew()
        {
            _source.ProductSource = null;
            return this;
        }

        public FactoryBinder<TProduct> FromMethod(Func<IResolver, TProduct> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _source.ProductSource = new DelegateActivatorSource(resolver => factory(resolver));
            return this;
        }

        public FactoryBinder<TProduct> FromMethod(Func<TProduct> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _source.ProductSource = new DelegateActivatorSource(resolver => factory());
            return this;
        }

        public FactoryBinder<TProduct> FromInstance(TProduct instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _source.ProductSource = new InstanceActivatorSource(instance);
            return this;
        }

        #endregion
    }
}

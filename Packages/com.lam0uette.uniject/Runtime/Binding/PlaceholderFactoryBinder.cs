using System;

namespace LaM0uette.UniJect
{
    public sealed class PlaceholderFactoryBinder<TFactory, TProduct>
    {
        #region Statements

        private readonly PlaceholderFactoryActivatorSource _source;

        public BindingDraft Draft { get; }

        public IProductSourceHolder Product
        {
            get { return _source; }
        }

        internal PlaceholderFactoryBinder(BindingDraft draft, PlaceholderFactoryActivatorSource source)
        {
            Draft = draft;
            _source = source;
        }

        #endregion

        #region Methods

        public PlaceholderFactoryBinder<TFactory, TProduct> AsSingleton()
        {
            Draft.SetLifetime(Lifetime.Singleton);
            return this;
        }

        public PlaceholderFactoryBinder<TFactory, TProduct> AsTransient()
        {
            Draft.SetLifetime(Lifetime.Transient);
            return this;
        }

        public PlaceholderFactoryBinder<TFactory, TProduct> NonLazy()
        {
            Draft.NonLazy = true;
            return this;
        }

        public PlaceholderFactoryBinder<TFactory, TProduct> WithId(object id)
        {
            Draft.Id = id;
            return this;
        }

        public PlaceholderFactoryBinder<TFactory, TProduct> FromMethod(Func<IResolver, TProduct> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _source.ProductSource = new DelegateActivatorSource(resolver => factory(resolver));
            return this;
        }

        #endregion
    }
}

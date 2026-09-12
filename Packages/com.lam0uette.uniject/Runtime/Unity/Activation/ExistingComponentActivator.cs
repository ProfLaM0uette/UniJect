using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class ExistingComponentActivator : IActivator
    {
        #region Statements

        private static readonly InjectionParameter[] NO_DEPENDENCIES = new InjectionParameter[0];

        private readonly GameObject _host;
        private readonly bool _includeChildren;
        private readonly bool _includeInactive;
        private readonly IInjector _injector;

        public Type ProducedType { get; }

        public Ownership Ownership
        {
            get { return Ownership.None; }
        }

        public IReadOnlyList<InjectionParameter> DeclaredDependencies
        {
            get { return NO_DEPENDENCIES; }
        }

        public ExistingComponentActivator(
            Type producedType,
            GameObject host,
            bool includeChildren,
            bool includeInactive,
            IInjector injector)
        {
            ProducedType = producedType ?? throw new ArgumentNullException(nameof(producedType));
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _includeChildren = includeChildren;
            _includeInactive = includeInactive;
            _injector = injector ?? throw new ArgumentNullException(nameof(injector));
        }

        #endregion

        #region Methods

        public object Create(in ResolutionContext context)
        {
            object found = UnityObjectFinder.FindOn(_host, ProducedType, _includeChildren, _includeInactive);

            if (found == null)
                throw new BindingNotFoundException(ProducedType, null, context.Path, null);

            return found;
        }

        public void Inject(object instance, in ResolutionContext context)
        {
            _injector.Inject(instance, in context);
        }

        #endregion
    }
}

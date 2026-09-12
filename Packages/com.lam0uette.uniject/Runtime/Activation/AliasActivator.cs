using System;
using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class AliasActivator : IActivator
    {
        #region Statements

        private static readonly InjectionParameter[] NO_DEPENDENCIES = new InjectionParameter[0];

        private readonly ServiceIdentifier _target;

        public Type ProducedType { get; }

        public Ownership Ownership
        {
            get { return Ownership.None; }
        }

        public IReadOnlyList<InjectionParameter> DeclaredDependencies
        {
            get { return NO_DEPENDENCIES; }
        }

        public ServiceIdentifier Target
        {
            get { return _target; }
        }

        public AliasActivator(ServiceIdentifier target, Type producedType)
        {
            _target = target;
            ProducedType = producedType ?? throw new ArgumentNullException(nameof(producedType));
        }

        #endregion

        #region Methods

        public object Create(in ResolutionContext context)
        {
            ResolutionRequest request = new ResolutionRequest(
                _target,
                context.Request.ConsumerType,
                context.Request.ConsumerInstance,
                context.Request.MemberName,
                context.Request.SiteKind);

            return context.ResolveDependency(in request);
        }

        public void Inject(object instance, in ResolutionContext context)
        {
        }

        #endregion
    }
}

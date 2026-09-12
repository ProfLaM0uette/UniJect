using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public readonly struct ResolutionContext : IResolutionContext
    {
        #region Statements

        private readonly DIContainer _container;

        public ResolutionRequest Request { get; }
        public IReadOnlyList<object> Arguments { get; }
        public int Depth { get; }

        public IResolver Resolver
        {
            get { return _container; }
        }

        public IInstantiator Instantiator
        {
            get { return _container; }
        }

        internal ResolutionPath Path { get; }

        internal DIContainer Container
        {
            get { return _container; }
        }

        internal ResolutionContext(
            DIContainer container,
            in ResolutionRequest request,
            IReadOnlyList<object> arguments,
            int depth,
            ResolutionPath path)
        {
            _container = container;
            Request = request;
            Arguments = arguments;
            Depth = depth;
            Path = path;
        }

        #endregion

        #region Methods

        public object ResolveDependency(in ResolutionRequest request)
        {
            return _container.ResolveRequest(in request, Path, Depth + 1);
        }

        public bool TryResolveDependency(in ResolutionRequest request, out object instance)
        {
            return _container.TryResolveRequest(in request, Path, Depth + 1, out instance);
        }

        #endregion
    }
}

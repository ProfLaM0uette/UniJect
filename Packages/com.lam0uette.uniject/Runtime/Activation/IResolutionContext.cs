using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public interface IResolutionContext
    {
        IResolver Resolver { get; }
        IInstantiator Instantiator { get; }
        ResolutionRequest Request { get; }
        IReadOnlyList<object> Arguments { get; }
        int Depth { get; }

        object ResolveDependency(in ResolutionRequest request);

        bool TryResolveDependency(in ResolutionRequest request, out object instance);
    }
}

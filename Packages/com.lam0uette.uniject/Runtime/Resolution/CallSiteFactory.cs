using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal sealed class CallSiteFactory
    {
        #region Statements

        private readonly DIContainer _container;

        private readonly Dictionary<ResolutionCacheKey, CallSite> _cache =
            new Dictionary<ResolutionCacheKey, CallSite>();

        public CallSiteFactory(DIContainer container)
        {
            _container = container;
        }

        #endregion

        #region Methods

        public CallSite GetCallSite(in ResolutionRequest request, CallSiteChain chain, bool throwOnMissing)
        {
            ResolutionCacheKey key = new ResolutionCacheKey(
                request.ContractType,
                request.Id,
                request.ConsumerType);

            if (_cache.TryGetValue(key, out CallSite cached))
                return cached;

            DIContainer owner = null;
            RegistrationGroup group = null;

            for (DIContainer current = _container; current != null; current = current.Parent)
            {
                if (!current.TryGetGroup(request.Identifier, out group))
                    continue;

                owner = current;
                break;
            }

            if (owner == null)
            {
                if (throwOnMissing)
                    throw new BindingNotFoundException(request.ContractType, request.Id, Snapshot(chain), null);

                return null;
            }

            List<SelectionFailure> rejected = new List<SelectionFailure>();
            Registration winner = RegistrationSelector.Select(group, in request, rejected, out List<Registration> ambiguous);

            if (ambiguous != null)
                throw new AmbiguousBindingException(request.ContractType, request.Id, Snapshot(chain), ambiguous);

            if (winner == null)
            {
                if (throwOnMissing)
                    throw new BindingNotFoundException(request.ContractType, request.Id, Snapshot(chain), rejected);

                return null;
            }

            CallSite site = new CallSite(winner, group.AllConditionsStatic);

            if (site.IsCacheable)
                _cache[key] = site;

            return site;
        }

        public void Clear()
        {
            _cache.Clear();
        }

        private static ResolutionPath Snapshot(CallSiteChain chain)
        {
            return chain == null ? new ResolutionPath() : chain.Path.Snapshot();
        }

        #endregion
    }
}

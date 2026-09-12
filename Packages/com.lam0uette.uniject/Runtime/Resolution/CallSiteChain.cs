using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    internal sealed class CallSiteChain
    {
        #region Statements

        private readonly List<ServiceIdentifier> _identifiers = new List<ServiceIdentifier>();
        private readonly List<InjectionSiteKind> _edgeKinds = new List<InjectionSiteKind>();
        private readonly List<Lifetime> _lifetimes = new List<Lifetime>();
        private readonly ResolutionPath _path = new ResolutionPath();

        public ResolutionPath Path
        {
            get { return _path; }
        }

        public int Count
        {
            get { return _identifiers.Count; }
        }

        #endregion

        #region Methods

        public bool Contains(in ServiceIdentifier identifier)
        {
            return IndexOf(in identifier) >= 0;
        }

        public void Push(in ResolutionRequest request, Lifetime lifetime)
        {
            _identifiers.Add(request.Identifier);
            _edgeKinds.Add(request.SiteKind);
            _lifetimes.Add(lifetime);
            _path.Push(request.ToFrame());
        }

        public void Pop()
        {
            int last = _identifiers.Count - 1;

            if (last < 0)
                return;

            _identifiers.RemoveAt(last);
            _edgeKinds.RemoveAt(last);
            _lifetimes.RemoveAt(last);
            _path.Pop();
        }

        public bool IsBreakableCycle(in ServiceIdentifier identifier, InjectionSiteKind edgeKind, Lifetime lifetime)
        {
            int start = IndexOf(in identifier);

            if (start < 0)
                return false;

            if (!IsCacheable(lifetime))
                return false;

            if (IsMemberEdge(edgeKind))
                return true;

            for (int i = start; i < _edgeKinds.Count; i++)
            {
                if (IsMemberEdge(_edgeKinds[i]) && IsCacheable(_lifetimes[i]))
                    return true;
            }

            return false;
        }

        private int IndexOf(in ServiceIdentifier identifier)
        {
            for (int i = 0; i < _identifiers.Count; i++)
            {
                if (_identifiers[i].Equals(identifier))
                    return i;
            }

            return -1;
        }

        private static bool IsMemberEdge(InjectionSiteKind kind)
        {
            return kind != InjectionSiteKind.Constructor;
        }

        private static bool IsCacheable(Lifetime lifetime)
        {
            return lifetime != Lifetime.Transient;
        }

        #endregion
    }
}

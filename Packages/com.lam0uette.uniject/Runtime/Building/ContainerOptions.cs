using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public sealed class ContainerOptions
    {
        #region Statements

        private const int DEFAULT_MAX_RESOLUTION_DEPTH = 256;

        public static ContainerOptions Strict
        {
            get
            {
                return new ContainerOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true,
                    AssertMainThread = true
                };
            }
        }

        public static ContainerOptions Relaxed
        {
            get
            {
                return new ContainerOptions
                {
                    ValidateOnBuild = false,
                    ValidateScopes = false,
                    AssertMainThread = false
                };
            }
        }

        public static ContainerOptions Default
        {
            get
            {
#if UNITY_EDITOR || DEBUG
                return Strict;
#else
                return Relaxed;
#endif
            }
        }

        public bool ValidateOnBuild { get; set; }
        public bool ValidateScopes { get; set; }
        public bool AssertMainThread { get; set; }
        public int MaxResolutionDepth { get; set; } = DEFAULT_MAX_RESOLUTION_DEPTH;
        public IInjector Injector { get; set; }
        public IInstanceLivenessPolicy Liveness { get; set; }
        public IResolutionObserver Observer { get; set; }
        public IReadOnlyList<IInstanceReleaser> Releasers { get; set; }

        #endregion

        #region Methods

        public IInjector ResolveInjector()
        {
            return Injector ?? new ReflectionInjector();
        }

        public IInstanceLivenessPolicy ResolveLiveness()
        {
            return Liveness ?? NullLivenessPolicy.Instance;
        }

        public IResolutionObserver ResolveObserver()
        {
            return Observer ?? NullResolutionObserver.Instance;
        }

        public IReadOnlyList<IInstanceReleaser> ResolveReleasers()
        {
            return Releasers ?? new IInstanceReleaser[] { DisposableReleaser.Instance };
        }

        #endregion
    }
}

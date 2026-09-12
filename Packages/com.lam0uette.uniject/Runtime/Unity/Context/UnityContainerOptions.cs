namespace LaM0uette.UniJect
{
    public static class UnityContainerOptions
    {
        public static IResolutionObserver Observer { get; set; }

        public static ContainerOptions Create()
        {
            ContainerOptions options = ContainerOptions.Default;
            options.Observer = Observer;
            options.Liveness = UnityLivenessPolicy.Instance;
            options.Releasers = new IInstanceReleaser[] { DisposableReleaser.Instance, UnityObjectReleaser.Instance };
            options.DefaultSourceRule = ComponentDefaultSourceRule.Instance;
            options.TickRegistry = TickRegistry.Instance;

            return options;
        }
    }
}

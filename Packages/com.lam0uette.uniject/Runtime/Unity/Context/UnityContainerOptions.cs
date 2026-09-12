namespace LaM0uette.UniJect
{
    public static class UnityContainerOptions
    {
        public static ContainerOptions Create()
        {
            ContainerOptions options = ContainerOptions.Default;
            options.Liveness = UnityLivenessPolicy.Instance;
            options.Releasers = new IInstanceReleaser[] { DisposableReleaser.Instance, UnityObjectReleaser.Instance };
            options.DefaultSourceRule = ComponentDefaultSourceRule.Instance;

            return options;
        }
    }
}

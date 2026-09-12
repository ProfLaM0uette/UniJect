namespace LaM0uette.UniJect
{
    internal static class UniJectStatics
    {
        public static void Reset()
        {
            SceneScopeRegistry.Reset();
            ProjectContext.Reset();
            PlayerLoopTickPump.Reset();
            SceneInjector.Reset();
            InactiveInstantiationScope.Reset();
            UnityObjectFinder.Reset();
            MainThreadGuard.Capture();
        }
    }
}

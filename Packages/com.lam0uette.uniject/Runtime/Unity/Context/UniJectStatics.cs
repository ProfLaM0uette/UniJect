namespace LaM0uette.UniJect
{
    internal static class UniJectStatics
    {
        public static void Reset()
        {
            SceneScopeRegistry.Reset();
            ProjectContext.Reset();
            SceneInjector.Reset();
            MainThreadGuard.Capture();
        }
    }
}

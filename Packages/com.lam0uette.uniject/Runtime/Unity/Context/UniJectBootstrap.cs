using UnityEngine;

namespace LaM0uette.UniJect
{
    internal static class UniJectBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            UniJectStatics.Reset();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateProjectContext()
        {
            ProjectContext.Initialize();
        }
    }
}

using UnityEngine;

namespace LaM0uette.UniJect
{
    public abstract class ScriptableObjectInstaller : ScriptableObject, IInstaller
    {
        public abstract void Install(IContainerBuilder builder);
    }
}

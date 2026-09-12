using UnityEngine;

namespace LaM0uette.UniJect
{
    public interface IGameObjectFactory
    {
        bool OwnsTheGameObject { get; }

        GameObject Provide(string defaultName);
    }
}

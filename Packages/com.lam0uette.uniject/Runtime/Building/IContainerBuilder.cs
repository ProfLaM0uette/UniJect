using System;

namespace LaM0uette.UniJect
{
    public interface IContainerBuilder
    {
        BuildPhase Phase { get; }

        void Add(Registration registration);

        bool TryAdd(Registration registration);

        void OnBuilt(Action<DIContainer> callback);

        DIContainer Build(ContainerOptions options);
    }
}

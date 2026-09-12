using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class GameObjectPlacement
    {
        public string Name { get; }
        public Transform Parent { get; }
        public ParentMode ParentMode { get; }
        public bool DontDestroyOnLoad { get; }
        public TransformSpace Space { get; }
        public Vector3? Position { get; }
        public Quaternion? Rotation { get; }
        public Vector3? Scale { get; }

        public GameObjectPlacement(
            string name,
            Transform parent,
            ParentMode parentMode,
            bool dontDestroyOnLoad,
            TransformSpace space,
            Vector3? position,
            Quaternion? rotation,
            Vector3? scale)
        {
            Name = name;
            Parent = parent;
            ParentMode = parentMode;
            DontDestroyOnLoad = dontDestroyOnLoad;
            Space = space;
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }
}

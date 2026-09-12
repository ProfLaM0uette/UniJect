using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class GameObjectPlacementDraft
    {
        #region Statements

        public string Name { get; set; }
        public Transform Parent { get; set; }
        public ParentMode ParentMode { get; set; }
        public bool DontDestroyOnLoad { get; set; }
        public TransformSpace Space { get; set; }
        public Vector3? Position { get; set; }
        public Quaternion? Rotation { get; set; }
        public Vector3? Scale { get; set; }

        #endregion

        #region Methods

        public void Apply(BindTransform transform)
        {
            if (transform == null)
                return;

            Space = transform.Space;

            if (transform.Position.HasValue)
                Position = transform.Position;

            if (transform.Rotation.HasValue)
                Rotation = transform.Rotation;

            if (transform.Scale.HasValue)
                Scale = transform.Scale;
        }

        public GameObjectPlacement ToPlacement()
        {
            return new GameObjectPlacement(
                Name,
                Parent,
                ParentMode,
                DontDestroyOnLoad,
                Space,
                Position,
                Rotation,
                Scale);
        }

        #endregion
    }
}

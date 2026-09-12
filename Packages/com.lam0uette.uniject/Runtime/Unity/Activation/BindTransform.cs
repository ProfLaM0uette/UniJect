using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class BindTransform
    {
        #region Statements

        public TransformSpace Space { get; set; }
        public Vector3? Position { get; set; }
        public Quaternion? Rotation { get; set; }
        public Vector3? Scale { get; set; }

        public BindTransform()
        {
        }

        public BindTransform(Transform transform)
        {
            if (transform == null)
                return;

            Space = TransformSpace.World;
            Position = transform.position;
            Rotation = transform.rotation;
            Scale = transform.localScale;
        }

        public BindTransform(Vector3? position, Quaternion? rotation, Vector3? scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        #endregion

        #region Methods

        public static implicit operator BindTransform(Transform transform)
        {
            return new BindTransform(transform);
        }

        #endregion
    }
}

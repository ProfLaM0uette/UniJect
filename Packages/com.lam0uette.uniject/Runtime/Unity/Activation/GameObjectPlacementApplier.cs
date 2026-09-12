using UnityEngine;

namespace LaM0uette.UniJect
{
    internal static class GameObjectPlacementApplier
    {
        public static void Apply(GameObject host, GameObjectPlacement placement, string fallbackName)
        {
            Transform transform = host.transform;

            if (placement.Scale.HasValue)
                transform.localScale = placement.Scale.Value;

            transform.SetParent(placement.ParentMode == ParentMode.Explicit ? placement.Parent : null, false);

            ApplyPose(transform, placement);

            host.name = placement.Name ?? fallbackName;
        }

        public static bool ShouldDontDestroyOnLoad(GameObjectPlacement placement)
        {
            return placement.DontDestroyOnLoad;
        }

        public static void ApplyDontDestroyOnLoad(GameObject host)
        {
            Object.DontDestroyOnLoad(host.transform.root.gameObject);
        }


        private static void ApplyPose(Transform transform, GameObjectPlacement placement)
        {
            if (placement.Space == TransformSpace.World)
            {
                if (placement.Position.HasValue)
                    transform.position = placement.Position.Value;

                if (placement.Rotation.HasValue)
                    transform.rotation = placement.Rotation.Value;

                return;
            }

            if (placement.Position.HasValue)
                transform.localPosition = placement.Position.Value;

            if (placement.Rotation.HasValue)
                transform.localRotation = placement.Rotation.Value;
        }
    }
}

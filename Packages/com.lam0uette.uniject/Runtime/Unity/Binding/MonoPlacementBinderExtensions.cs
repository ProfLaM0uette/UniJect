using UnityEngine;

namespace LaM0uette.UniJect
{
    public static class MonoPlacementBinderExtensions
    {
        public static Binder<TContract, TConcrete> Name<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            string name)
            where TConcrete : Component, TContract
        {
            GameObjectSourceAccess.GetOrCreate(binder.Draft).Placement.Name = name;
            return binder;
        }

        public static Binder<TContract, TConcrete> Parent<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            Transform parent)
            where TConcrete : Component, TContract
        {
            GameObjectPlacementDraft placement = GameObjectSourceAccess.GetOrCreate(binder.Draft).Placement;
            placement.Parent = parent;
            placement.ParentMode = ParentMode.Explicit;

            return binder;
        }

        public static Binder<TContract, TConcrete> Root<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder)
            where TConcrete : Component, TContract
        {
            GameObjectPlacementDraft placement = GameObjectSourceAccess.GetOrCreate(binder.Draft).Placement;
            placement.Parent = null;
            placement.ParentMode = ParentMode.SceneRoot;

            return binder;
        }

        public static Binder<TContract, TConcrete> DontDestroyOnLoad<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder)
            where TConcrete : Component, TContract
        {
            GameObjectSourceAccess.GetOrCreate(binder.Draft).Placement.DontDestroyOnLoad = true;
            return binder;
        }

        public static Binder<TContract, TConcrete> Transform<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            BindTransform transform)
            where TConcrete : Component, TContract
        {
            GameObjectSourceAccess.GetOrCreate(binder.Draft).Placement.Apply(transform);
            return binder;
        }

        public static Binder<TContract, TConcrete> Position<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            Vector3 position)
            where TConcrete : Component, TContract
        {
            GameObjectSourceAccess.GetOrCreate(binder.Draft).Placement.Position = position;
            return binder;
        }

        public static Binder<TContract, TConcrete> Rotation<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            Quaternion rotation)
            where TConcrete : Component, TContract
        {
            GameObjectSourceAccess.GetOrCreate(binder.Draft).Placement.Rotation = rotation;
            return binder;
        }

        public static Binder<TContract, TConcrete> Scale<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            Vector3 scale)
            where TConcrete : Component, TContract
        {
            GameObjectSourceAccess.GetOrCreate(binder.Draft).Placement.Scale = scale;
            return binder;
        }
    }
}

using UnityEngine;

namespace LaM0uette.UniJect
{
    public static class PlaceholderFactoryPlacementExtensions
    {
        public static PlaceholderFactoryBinder<TFactory, TProduct> Name<TFactory, TProduct>(
            this PlaceholderFactoryBinder<TFactory, TProduct> binder,
            string name)
            where TProduct : Component
        {
            GameObjectSourceAccess.GetOrCreate(binder.Product).Placement.Name = name;
            return binder;
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> Parent<TFactory, TProduct>(
            this PlaceholderFactoryBinder<TFactory, TProduct> binder,
            Transform parent)
            where TProduct : Component
        {
            GameObjectPlacementDraft placement = GameObjectSourceAccess.GetOrCreate(binder.Product).Placement;
            placement.Parent = parent;
            placement.ParentMode = ParentMode.Explicit;

            return binder;
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> Root<TFactory, TProduct>(
            this PlaceholderFactoryBinder<TFactory, TProduct> binder)
            where TProduct : Component
        {
            GameObjectPlacementDraft placement = GameObjectSourceAccess.GetOrCreate(binder.Product).Placement;
            placement.Parent = null;
            placement.ParentMode = ParentMode.SceneRoot;

            return binder;
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> DontDestroyOnLoad<TFactory, TProduct>(
            this PlaceholderFactoryBinder<TFactory, TProduct> binder)
            where TProduct : Component
        {
            GameObjectSourceAccess.GetOrCreate(binder.Product).Placement.DontDestroyOnLoad = true;
            return binder;
        }

        public static PlaceholderFactoryBinder<TFactory, TProduct> FromNewPrefab<TFactory, TProduct>(
            this PlaceholderFactoryBinder<TFactory, TProduct> binder,
            GameObject prefab)
            where TProduct : Component
        {
            GameObjectSource source = GameObjectSourceAccess.GetOrCreate(binder.Product);
            source.Factory = new PrefabGameObjectFactory(prefab);
            source.FromPrefab = true;

            return binder;
        }
    }
}

using UnityEngine;

namespace LaM0uette.UniJect
{
    public static class MonoSourceBinderExtensions
    {
        public static Binder<TContract, TConcrete> FromNewGameObject<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder)
            where TConcrete : Component, TContract
        {
            GameObjectSource source = GameObjectSourceAccess.GetOrCreate(binder.Draft);
            source.Factory = new NewGameObjectFactory();
            source.FromPrefab = false;

            return binder;
        }

        public static Binder<TContract, TConcrete> FromNewPrefab<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            GameObject prefab)
            where TConcrete : Component, TContract
        {
            GameObjectSource source = GameObjectSourceAccess.GetOrCreate(binder.Draft);
            source.Factory = new PrefabGameObjectFactory(prefab);
            source.FromPrefab = true;

            return binder;
        }

        public static Binder<TContract, TConcrete> FromNewPrefabResource<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            string resourcePath)
            where TConcrete : Component, TContract
        {
            GameObjectSource source = GameObjectSourceAccess.GetOrCreate(binder.Draft);
            source.Factory = new ResourcePrefabGameObjectFactory(resourcePath);
            source.FromPrefab = true;

            return binder;
        }

        public static Binder<TContract, TConcrete> FromNewComponentOnGameObject<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            GameObject host)
            where TConcrete : Component, TContract
        {
            GameObjectSource source = GameObjectSourceAccess.GetOrCreate(binder.Draft);
            source.Factory = new ExistingGameObjectFactory(host);
            source.FromPrefab = false;

            return binder;
        }

        public static Binder<TContract, TConcrete> FromComponentInHierarchy<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            bool includeInactive = false)
            where TConcrete : Component, TContract
        {
            binder.Draft.SetSource(new HierarchyComponentSource(includeInactive));
            return binder;
        }

        public static Binder<TContract, TConcrete> FromComponentOnGameObject<TContract, TConcrete>(
            this Binder<TContract, TConcrete> binder,
            GameObject host,
            bool includeChildren = false,
            bool includeInactive = false)
            where TConcrete : Component, TContract
        {
            binder.Draft.SetSource(new ExistingComponentSource(host, includeChildren, includeInactive));
            return binder;
        }
    }
}

using LaM0uette.UniJect;
using UnityEngine;

namespace UniJect.Demo
{
    public sealed class DemoSpawnInstaller : MonoInstaller
    {
        #region Statements

        [SerializeField] private Transform _spawnRoot;

        #endregion

        #region Methods

        public override void Install()
        {
            Container.Bind<Spinner>()
                .FromNewGameObject()
                .Name("Spinner (made by the container)")
                .Parent(_spawnRoot)
                .Position(new Vector3(0f, 1.5f, 0f))
                .AsSingleton()
                .NonLazy();

            Container.BindPlaceholderFactoryTo<ProjectileFactory, CriticalProjectileFactory, Projectile, int>();
            Container.BindFactory<Projectile, int>();
        }

        #endregion
    }
}

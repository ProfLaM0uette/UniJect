using UnityEngine;

namespace LaM0uette.UniJect.Samples.GameObjects
{
    public sealed class TurretInstaller : MonoInstaller
    {
        #region Statements

        [SerializeField] private GameObject _turretPrefab;
        [SerializeField] private Transform _turretRoot;

        #endregion

        #region Methods

        public override void Install()
        {
            Container.Bind<ITargeting, NearestTargeting>().AsSingleton();

            Container.Bind<Turret>()
                .FromNewPrefab(_turretPrefab)
                .Name("Turret")
                .Parent(_turretRoot)
                .AsSingleton()
                .NonLazy();
        }

        #endregion
    }
}

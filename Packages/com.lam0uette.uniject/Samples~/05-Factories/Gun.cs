using UnityEngine;

namespace LaM0uette.UniJect.Samples.Factories
{
    public sealed class Gun : MonoBehaviour
    {
        #region Statements

        [Inject] private BulletFactory _bullets;
        [Inject] private IFactory<Bullet, int> _plainBullets;

        private void Start()
        {
            Debug.Log("factory: " + _bullets.Create(5).Damage + ", plain: " + _plainBullets.Create(5).Damage);
        }

        #endregion
    }
}

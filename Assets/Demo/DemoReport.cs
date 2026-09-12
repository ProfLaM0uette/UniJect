using System;
using System.Text;
using LaM0uette.UniJect;
using UnityEngine;

namespace UniJect.Demo
{
    public sealed class DemoReport : MonoBehaviour
    {
        #region Statements

        [Inject] private IDemoLogger _logger;
        [Inject] private Knight _knight;
        [Inject] private Archer _archer;
        [Inject] private Referee _referee;
        [Inject("backup")] private IWeapon _backupWeapon;
        [Inject] private ProjectileFactory _criticals;
        [Inject] private IFactory<Projectile, int> _projectiles;
        [Inject] private Spinner _spinner;
        [Inject] private DIContainer _container;

        [InjectOptional] private IDisposable _nothingIsBoundToThis;

        private void Start()
        {
            _logger.Line(Report());
        }

        #endregion

        #region Methods

        private string Report()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine("interface binding      IDemoLogger -> " + _logger.GetType().Name);
            builder.AppendLine("unconditional          Knight got the " + _knight.Weapon.Name);
            builder.AppendLine("WhenInjectedInto       Archer got the " + _archer.Weapon.Name);
            builder.AppendLine("WithId(\"backup\")       " + _backupWeapon.Name);
            builder.AppendLine("collection             Referee sees " + _referee.RuleCount() +
                               " rules, total " + _referee.Total());
            builder.AppendLine("factory                plain Create(10) -> damage " +
                               _projectiles.Create(10).Damage);
            builder.AppendLine("placeholder factory    overridden Create(10) -> damage " +
                               _criticals.Create(10).Damage);
            builder.AppendLine("FromNewGameObject      " + _spinner.gameObject.name + ", parent " +
                               _spinner.transform.parent.name);
            builder.AppendLine("injected before Awake  " + _spinner.SawLoggerInAwake);
            builder.AppendLine("InjectOptional         unbound IDisposable left as " +
                               (_nothingIsBoundToThis == null ? "null" : "something"));
            builder.AppendLine("TryResolve             unbound IComparable found: " +
                               _container.TryResolve(typeof(IComparable), out object _));
            builder.Append("self binding           resolved the container itself: " +
                           (_container != null));

            return builder.ToString();
        }

        #endregion
    }
}

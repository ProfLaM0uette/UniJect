using System.Collections.Generic;
using UnityEngine;

namespace LaM0uette.UniJect
{
    [CreateAssetMenu(
        fileName = "ProjectContextSettings",
        menuName = "UniJect/Project Context Settings",
        order = 0)]
    public sealed class ProjectContextSettings : ScriptableObject
    {
        private static readonly ScriptableObjectInstaller[] EMPTY = new ScriptableObjectInstaller[0];

        [SerializeField] private ScriptableObjectInstaller[] _installers;

        public IReadOnlyList<ScriptableObjectInstaller> Installers
        {
            get { return _installers ?? EMPTY; }
        }
    }
}

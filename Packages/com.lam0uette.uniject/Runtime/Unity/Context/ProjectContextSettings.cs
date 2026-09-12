using System.Collections.Generic;
using UnityEngine;

namespace LaM0uette.UniJect
{
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

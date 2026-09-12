using System;
using UnityEngine;

namespace LaM0uette.UniJect
{
    public sealed class ComponentDefaultSourceRule : IDefaultSourceRule
    {
        #region Statements

        public static readonly ComponentDefaultSourceRule Instance = new ComponentDefaultSourceRule();

        private ComponentDefaultSourceRule()
        {
        }

        #endregion

        #region Methods

        public IActivatorSource Resolve(Type concreteType)
        {
            if (concreteType == null || !typeof(Component).IsAssignableFrom(concreteType))
                return null;

            return new GameObjectSource();
        }

        #endregion
    }
}

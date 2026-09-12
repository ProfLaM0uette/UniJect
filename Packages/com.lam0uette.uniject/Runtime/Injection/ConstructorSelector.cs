using System;
using System.Collections.Generic;
using System.Reflection;

namespace LaM0uette.UniJect
{
    internal static class ConstructorSelector
    {
        #region Statements

        private const string COMPONENT_TYPE_NAME = "UnityEngine.Component";
        private const string SCRIPTABLE_OBJECT_TYPE_NAME = "UnityEngine.ScriptableObject";

        private const BindingFlags CONSTRUCTOR_FLAGS =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        #endregion

        #region Methods

        public static ConstructorInfo Select(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsAbstract || type.IsInterface || IsEngineConstructed(type))
                return null;

            ConstructorInfo[] constructors = type.GetConstructors(CONSTRUCTOR_FLAGS);

            if (constructors.Length == 0)
                return null;

            if (constructors.Length == 1)
                return constructors[0];

            List<ConstructorInfo> marked = new List<ConstructorInfo>();
            List<ConstructorInfo> publics = new List<ConstructorInfo>();

            for (int i = 0; i < constructors.Length; i++)
            {
                ConstructorInfo constructor = constructors[i];

                if (constructor.IsDefined(typeof(InjectAttributeBase), false))
                    marked.Add(constructor);

                if (constructor.IsPublic)
                    publics.Add(constructor);
            }

            if (marked.Count == 1)
                return marked[0];

            if (marked.Count > 1)
                throw new AmbiguousConstructorException(type, marked);

            if (publics.Count == 1)
                return publics[0];

            throw new AmbiguousConstructorException(type, constructors);
        }

        public static bool IsEngineConstructed(Type type)
        {
            for (Type current = type; current != null; current = current.BaseType)
            {
                if (current.FullName == COMPONENT_TYPE_NAME || current.FullName == SCRIPTABLE_OBJECT_TYPE_NAME)
                    return true;
            }

            return false;
        }

        #endregion
    }
}

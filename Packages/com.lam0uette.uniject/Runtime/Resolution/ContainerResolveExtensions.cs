using System.Collections.Generic;

namespace LaM0uette.UniJect
{
    public static class ContainerResolveExtensions
    {
        public static T Resolve<T>(this IResolver resolver)
        {
            return (T)resolver.Resolve(typeof(T));
        }

        public static T ResolveId<T>(this IResolver resolver, object id)
        {
            return (T)resolver.ResolveId(typeof(T), id);
        }

        public static bool TryResolve<T>(this IResolver resolver, out T instance)
        {
            if (resolver.TryResolve(typeof(T), out object resolved))
            {
                instance = (T)resolved;
                return true;
            }

            instance = default;
            return false;
        }

        public static bool TryResolveId<T>(this IResolver resolver, object id, out T instance)
        {
            if (resolver.TryResolveId(typeof(T), id, out object resolved))
            {
                instance = (T)resolved;
                return true;
            }

            instance = default;
            return false;
        }

        public static IReadOnlyList<T> ResolveAll<T>(this IResolver resolver)
        {
            IReadOnlyList<object> resolved = resolver.ResolveAll(typeof(T));
            List<T> instances = new List<T>(resolved.Count);

            for (int i = 0; i < resolved.Count; i++)
                instances.Add((T)resolved[i]);

            return instances;
        }

        public static bool HasBinding<T>(this IResolver resolver, object id = null)
        {
            return resolver.HasBinding(typeof(T), id);
        }

        public static T CreateInstance<T>(this IInstantiator instantiator)
        {
            return (T)instantiator.CreateInstance(typeof(T));
        }

        public static T CreateInstance<T>(this IInstantiator instantiator, IReadOnlyList<object> arguments)
        {
            return (T)instantiator.CreateInstance(typeof(T), arguments);
        }
    }
}

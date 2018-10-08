using System;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    internal static class TypeUtils
    {
        public static IEnumerable<Type> GetShallowAssignableTypes(this Type parent, IEnumerable<Type> types)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            foreach (var child in types)
            {
                if (parent != child && child.IsAssignableFrom(parent))
                {
                    yield return child;
                }
            }
        }

        public static IEnumerable<Type> GetDeepAssignableTypes(this Type parent, IEnumerable<Type> types)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            var assignableTypes = new HashSet<Type>(GetShallowAssignableTypes(parent, types));
            var newAssignableTypes = new HashSet<Type>();
            var loop = true;

            while (loop)
            {
                foreach (var type in assignableTypes)
                {
                    foreach (var heir in GetShallowAssignableTypes(type, assignableTypes))
                    {
                        newAssignableTypes.Add(heir);
                    }
                }

                loop = assignableTypes.Count != newAssignableTypes.Count ||
                    newAssignableTypes.Any(q => !assignableTypes.Contains(q)) ||
                    assignableTypes.Any(q => !newAssignableTypes.Contains(q));

                assignableTypes = new HashSet<Type>(newAssignableTypes);
            }

            return assignableTypes.AsEnumerable();
        }
    }
}

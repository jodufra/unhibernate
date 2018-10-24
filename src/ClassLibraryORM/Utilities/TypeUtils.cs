using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace System
{
    internal static class TypeUtils
    {
        private static HashSet<Type> actionTypes = new HashSet<Type>
        {
            typeof(Action),
            typeof(Action<>),
            typeof(Action<,>),
            typeof(Action<,,>),
            typeof(Action<,,,>),
            typeof(Action<,,,,>),
            typeof(Action<,,,,,>),
            typeof(Action<,,,,,,>),
            typeof(Action<,,,,,,,>),
            typeof(Action<,,,,,,,,>),
            typeof(Action<,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,,,,,>)
        };

        private static HashSet<Type> expressionTypes = new HashSet<Type>
        {
            typeof(Expression),
            typeof(Expression<>)
        };

        private static HashSet<Type> funcTypes = new HashSet<Type>
        {
            typeof(Func<>),
            typeof(Func<,>),
            typeof(Func<,,>),
            typeof(Func<,,,>),
            typeof(Func<,,,,>),
            typeof(Func<,,,,,>),
            typeof(Func<,,,,,,>),
            typeof(Func<,,,,,,,>),
            typeof(Func<,,,,,,,,>),
            typeof(Func<,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,,,,>)
        };

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

        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static bool IsDelegateType(this Type type)
        {
            return typeof(Delegate).IsAssignableFrom(type?.BaseType ?? type);
        }

        public static bool IsAction(this Type type)
        {
            Type generic;
            if (type.IsGenericType)
            {
                generic = type.GetGenericTypeDefinition();
            }
            else
            {
                generic = type;
            }
            return actionTypes.Any(q => q == generic);
        }

        public static bool IsExpression(this Type type)
        {
            Type generic;
            if (type.IsGenericType)
            {
                generic = type.GetGenericTypeDefinition();
            }
            else
            {
                generic = type;
            }
            return expressionTypes.Any(q => q == generic);
        }

        public static bool IsFunc(this Type type)
        {
            Type generic;
            if (type.IsGenericType)
            {
                generic = type.GetGenericTypeDefinition();
            }
            else
            {
                generic = type;
            }
            return funcTypes.Any(q => q == generic);
        }
    }
}

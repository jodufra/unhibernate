using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

        public static bool IsActionType(this Type type)
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

        public static bool IsExpressionType(this Type type)
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

        public static bool IsFuncType(this Type type)
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

        /// <summary>
        /// Search for a method by name and parameter types.  
        /// Unlike GetMethod(), does 'loose' matching on generic
        /// parameter types, and searches base interfaces.
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        public static MethodInfo GetMethodExt(this Type thisType, string name, params Type[] parameterTypes)
        {
            return GetMethodExt(thisType, name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, parameterTypes);
        }

        /// <summary>
        /// Search for a method by name, parameter types, and binding flags.  
        /// Unlike GetMethod(), does 'loose' matching on generic
        /// parameter types, and searches base interfaces.
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        public static MethodInfo GetMethodExt(this Type thisType, string name, BindingFlags bindingFlags, params Type[] parameterTypes)
        {
            MethodInfo matchingMethod = null;

            // Check all methods with the specified name, including in base classes
            GetMethodExt(ref matchingMethod, thisType, name, bindingFlags, parameterTypes);

            // If we're searching an interface, we have to manually search base interfaces
            if (matchingMethod == null && thisType.IsInterface)
            {
                foreach (var interfaceType in thisType.GetInterfaces())
                {
                    GetMethodExt(ref matchingMethod, interfaceType, name, bindingFlags, parameterTypes);
                }
            }

            return matchingMethod;
        }

        private static void GetMethodExt(ref MethodInfo matchingMethod, Type type, string name, BindingFlags bindingFlags, params Type[] parameterTypes)
        {
            // Check all methods with the specified name, including in base classes
            foreach (MethodInfo methodInfo in type.GetMember(name, MemberTypes.Method, bindingFlags))
            {
                // Check that the parameter counts and types match, 
                // with 'loose' matching on generic parameters
                var parameterInfos = methodInfo.GetParameters();
                if (parameterInfos.Length == parameterTypes.Length)
                {
                    var i = 0;
                    for (; i < parameterInfos.Length; ++i)
                    {
                        if (!parameterInfos[i].ParameterType.IsSimilarType(parameterTypes[i]))
                        {
                            break;
                        }
                    }
                    if (i == parameterInfos.Length)
                    {
                        if (matchingMethod == null)
                        {
                            matchingMethod = methodInfo;
                        }
                        else
                        {
                            throw new AmbiguousMatchException("More than one matching method found!");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Special type used to match any generic parameter type in GetMethodExt().
        /// </summary>
        private class T { }

        /// <summary>
        /// Determines if the two types are either identical, or are both generic 
        /// parameters or generic types with generic parameters in the same
        ///  locations (generic parameters match any other generic paramter,
        /// but NOT concrete types).
        /// </summary>
        private static bool IsSimilarType(this Type thisType, Type type)
        {
            // Ignore any 'ref' types
            if (thisType.IsByRef)
            {
                thisType = thisType.GetElementType();
            }

            if (type.IsByRef)
            {
                type = type.GetElementType();
            }

            // Handle array types
            if (thisType.IsArray && type.IsArray)
            {
                return thisType.GetElementType().IsSimilarType(type.GetElementType());
            }

            // If the types are identical, or they're both generic parameters 
            // or the special 'T' type, treat as a match
            if (thisType == type || ((thisType.IsGenericParameter || thisType == typeof(T)) && (type.IsGenericParameter || type == typeof(T))))
            {
                return true;
            }

            // Handle any generic arguments
            if (thisType.IsGenericType && type.IsGenericType)
            {
                var thisArguments = thisType.GetGenericArguments();
                var arguments = type.GetGenericArguments();
                if (thisArguments.Length == arguments.Length)
                {
                    for (var i = 0; i < thisArguments.Length; ++i)
                    {
                        if (!thisArguments[i].IsSimilarType(arguments[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            return false;
        }
    }
}

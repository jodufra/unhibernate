using UnHibernate.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnHibernate.Managers
{
    public class TypeManager
    {
        public static TypeManager Instance { get; } = new TypeManager();

        private readonly Dictionary<Type, Type> pairs;
        private readonly Dictionary<string, Type> interfacesByName;
        private readonly Dictionary<string, InterfaceClassPair> pairsCache;

        private TypeManager()
        {
            pairs = new Dictionary<Type, Type>();
            interfacesByName = new Dictionary<string, Type>();
            pairsCache = new Dictionary<string, InterfaceClassPair>();
        }

        public InterfaceClassPair GetInterfaceClassPair<TInterface>() => GetInterfaceClassPair(typeof(TInterface));

        public InterfaceClassPair GetInterfaceClassPair(Type interfaze)
        {
            if (interfaze == null || !interfaze.IsInterface)
            {
                throw new InvalidCastException($"Parameter `{nameof(interfaze)}` is not an interface");
            }

            return GetInterfaceClassPair(interfaze.FullName);
        }

        public InterfaceClassPair GetInterfaceClassPair(string interfaceFullName)
        {
            if (!interfacesByName.ContainsKey(interfaceFullName))
            {
                throw new KeyNotFoundException($"Interface `{interfaceFullName}` is not mapped");
            }

            if (!pairsCache.ContainsKey(interfaceFullName))
            {
                var interfaceType = interfacesByName[interfaceFullName];
                var pair = InterfaceClassPair.CreateNew(interfaceType, pairs[interfaceType]);
                pairsCache.Add(interfaceFullName, pair);
            }

            return pairsCache[interfaceFullName];
        }

        public void Add<TInterface, TClass>() where TClass : class, TInterface => Add(typeof(TInterface), typeof(TClass));

        public void Add(Type interfaceType, Type classType)
        {
            var interfaceFullName = interfaceType.FullName;

            if (!interfacesByName.ContainsKey(interfaceFullName))
            {
                interfacesByName.Add(interfaceFullName, interfaceType);
            }

            var pair = InterfaceClassPair.CreateNew(interfaceType, classType);

            if (!pairsCache.ContainsKey(interfaceFullName))
            {
                pairs.Add(interfaceType, classType);
                pairsCache.Add(interfaceFullName, pair);
            }
            else
            {
                pairs[interfaceType] = classType;
                pairsCache[interfaceFullName] = pair;
            }
        }

        public TInterface Instantiate<TInterface>()
        {
            if (TryGetInterfaceClassPair<TInterface>(out var pair))
            {
                return (TInterface)FormatterServices.GetUninitializedObject(pair.Class);
            }

            return default(TInterface);
        }

        public bool TryGetInterfaceClassPair(string interfaceFullName, out InterfaceClassPair handler)
        {
            handler = InterfaceClassPair.Empty;

            try
            {
                handler = GetInterfaceClassPair(interfaceFullName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TryGetInterfaceClassPair(Type interfaze, out InterfaceClassPair handler)
        {
            handler = InterfaceClassPair.Empty;

            try
            {
                handler = GetInterfaceClassPair(interfaze);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TryGetInterfaceClassPair<TInterface>(out InterfaceClassPair handler)
        {
            handler = InterfaceClassPair.Empty;

            try
            {
                handler = GetInterfaceClassPair<TInterface>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TryAdd(Type interfaze, Type clazz)
        {
            try
            {
                Add(interfaze, clazz);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TryAdd<TInterface, TClass>() where TClass : class, TInterface
        {
            try
            {
                Add<TInterface, TClass>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

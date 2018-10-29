using System;
using System.Collections.Generic;
using System.Linq;

namespace UnHibernate.Collections
{
    public class InterfaceClassPairBuilder
    {
        private Type classType;
        private Type interfaceType;

        public InterfaceClassPairBuilder Class(Type classType)
        {
            this.classType = classType ?? throw new ArgumentNullException(nameof(classType));
            return this;
        }

        public InterfaceClassPairBuilder Interface(Type interfaceType)
        {
            this.interfaceType = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
            return this;
        }

        public InterfaceClassPair Build()
        {
            if (interfaceType == null || classType == null)
            {
                throw new InvalidOperationException($"The `{nameof(InterfaceClassPair)}` is not fully initialized.");
            }

            if (!interfaceType.IsInterface)
            {
                throw new InvalidOperationException($"The `{nameof(Interface)}` must be an interface.");
            }

            if (classType.IsInterface)
            {
                throw new InvalidOperationException($"The `{nameof(Class)}` must be a class.");
            }

            var constructor = classType.GetConstructor(Type.EmptyTypes);

            if (classType.IsAbstract || constructor == null)
            {
                throw new InvalidOperationException($"The `{nameof(Class)}` must be a class that provides a public parameterless constructor.");
            }

            if (classType.IsAssignableFrom(interfaceType))
            {
                throw new InvalidOperationException($"The class `{classType.FullName}` doesn't implement interface `{interfaceType.FullName}`.");
            }

            return new InterfaceClassPair(classType, interfaceType);
        }
    }
}

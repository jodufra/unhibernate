using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibraryORM.Collections
{
    public struct InterfaceClassPair : IEquatable<InterfaceClassPair>
    {
        private InterfaceClassPair(InterfaceClassPair pair) : this(pair.Class, pair.Interface)
        {
        }

        private InterfaceClassPair(Type @class, Type @interface)
        {
            Class = @class;
            Interface = @interface;
        }

        public Type Class { get; private set; }

        public Type Interface { get; private set; }

        public override bool Equals(object obj) => obj is InterfaceClassPair && Equals((InterfaceClassPair)obj);

        public bool Equals(InterfaceClassPair other) => EqualityComparer<Type>.Default.Equals(Class, other.Class) && EqualityComparer<Type>.Default.Equals(Interface, other.Interface);

        public override int GetHashCode()
        {
            var hashCode = 2088105799;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(Class);
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(Interface);
            return hashCode;
        }

        public static bool operator ==(InterfaceClassPair pair1, InterfaceClassPair pair2)
        {
            return pair1.Equals(pair2);
        }

        public static bool operator !=(InterfaceClassPair pair1, InterfaceClassPair pair2)
        {
            return !(pair1 == pair2);
        }

        public static InterfaceClassPair CreateNew(Type interfaceType, Type classType)
        {
            var b = new InterfaceClassPairBuilder().Interface(interfaceType).Class(classType);
            return b.Build();
        }

        public static readonly InterfaceClassPair Empty = new InterfaceClassPair { Class = null, Interface = null };

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
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace UnHibernate.Collections
{
    public struct InterfaceClassPair : IEquatable<InterfaceClassPair>
    {
        public InterfaceClassPair(Type @class, Type @interface)
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
    }
}

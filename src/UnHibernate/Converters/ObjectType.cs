using System;

namespace UnHibernate.Converters
{
    internal struct ObjectType
    {
        public ObjectType(object obj) : this()
        {
            Obj = obj ?? throw new ArgumentNullException(nameof(obj));
            Type = obj.GetType();
        }

        public object Obj { get; set; }
        public Type Type { get; set; }
    }
}

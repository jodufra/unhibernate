using System;

namespace UnHibernate.Converters.Objects
{
    internal class ConvertActionObject : ConvertDelegateObject
    {
        public ConvertActionObject(object instance, ObjectType objectType, Type targetType) : base(instance, objectType, targetType)
        {
            if (!objectType.Type.IsActionType())
            {
                throw new ArgumentOutOfRangeException(nameof(objectType));
            }
        }

        public override ObjectType Execute()
        {
            return base.Execute();
        }
    }
}

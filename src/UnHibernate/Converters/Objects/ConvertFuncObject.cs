using System;

namespace UnHibernate.Converters.Objects
{
    internal class ConvertFuncObject : ConvertDelegateObject
    {
        public ConvertFuncObject(object instance, ObjectType objectType, Type targetType) : base(instance, objectType, targetType)
        {
            if (!objectType.Type.IsFuncType())
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

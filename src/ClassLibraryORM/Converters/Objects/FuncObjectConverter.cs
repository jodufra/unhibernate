using System;

namespace ClassLibraryORM.Converters.Objects
{
    internal class FuncObjectConverter : DelegateObjectConverter
    {
        public FuncObjectConverter(object instance, ObjectType objectType, Type targetType) : base(instance, objectType, targetType)
        {
            if (!objectType.Type.IsFuncType())
            {
                throw new ArgumentOutOfRangeException(nameof(objectType));
            }
        }
    }
}

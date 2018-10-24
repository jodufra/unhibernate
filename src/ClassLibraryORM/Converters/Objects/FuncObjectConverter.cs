using System;

namespace ClassLibraryORM.Converters.Objects
{
    internal class FuncObjectConverter : DelegateObjectConverter
    {
        public FuncObjectConverter(ObjectType objectType, Type targetType) : base(objectType, targetType)
        {
            if (!objectType.Type.IsFunc())
            {
                throw new ArgumentOutOfRangeException(nameof(objectType));
            }
        }
    }
}

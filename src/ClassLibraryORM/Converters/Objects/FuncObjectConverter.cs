using System;

namespace ClassLibraryORM.Converters.Objects
{
    internal class FuncObjectConverter : ObjectConverter
    {
        public FuncObjectConverter(ObjectType objectType, Type targetType) : base(objectType, targetType)
        {
            if (!objectType.Type.IsFunc())
            {
                throw new ArgumentOutOfRangeException(nameof(objectType));
            }
        }

        public override ObjectType Convert()
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}

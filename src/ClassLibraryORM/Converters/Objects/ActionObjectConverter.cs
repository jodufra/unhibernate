using System;

namespace ClassLibraryORM.Converters.Objects
{
    internal class ActionObjectConverter : ObjectConverter
    {
        public ActionObjectConverter(ObjectType objectType, Type targetType) : base(objectType, targetType)
        {
            if (!objectType.Type.IsAction())
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

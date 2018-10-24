using System;

namespace ClassLibraryORM.Converters.Objects
{
    internal class ActionObjectConverter : DelegateObjectConverter
    {
        public ActionObjectConverter(ObjectType objectType, Type targetType) : base(objectType, targetType)
        {
            if (!objectType.Type.IsActionType())
            {
                throw new ArgumentOutOfRangeException(nameof(objectType));
            }
        }
    }
}

using System;

namespace ClassLibraryORM.Converters.Objects
{
    internal class ActionObjectConverter : DelegateObjectConverter
    {
        public ActionObjectConverter(object instance, ObjectType objectType, Type targetType) : base(instance, objectType, targetType)
        {
            if (!objectType.Type.IsActionType())
            {
                throw new ArgumentOutOfRangeException(nameof(objectType));
            }
        }
    }
}

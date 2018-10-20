using System;

namespace ClassLibraryORM.Converters.Objects
{
    internal class ObjectConverter
    {
        protected ObjectType objectType;
        protected Type targetType;

        public ObjectConverter(ObjectType objectType, Type targetType)
        {
            this.objectType = objectType;
            this.targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        }

        public virtual ObjectType Convert()
        {
            try
            {
                var obj = System.Convert.ChangeType(objectType.Obj, targetType);
                var newObjectType = new ObjectType(obj);
                return newObjectType;
            }
            catch (Exception)
            {
                // silent exception
                return objectType;
            }
        }
    }
}

using System;

namespace UnHibernate.Converters.Objects
{
    internal class ConvertObject
    {
        protected object instance;
        protected ObjectType objectType;
        protected Type targetType;

        public ConvertObject(object instance, ObjectType objectType, Type targetType)
        {
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
            this.objectType = objectType;
            this.targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        }

        public virtual ObjectType Execute()
        {
            try
            {
                var obj = System.Convert.ChangeType(objectType.Obj, targetType);
                return new ObjectType(obj);
            }
            catch (Exception)
            {
                // fallback by returning the original objectType
                return objectType;
            }
        }
    }
}

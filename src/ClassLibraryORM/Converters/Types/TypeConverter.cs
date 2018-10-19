using System;

namespace ClassLibraryORM.Converters.Types
{
    internal class TypeConverter
    {
        protected Type srcType;
        protected Type targetType;

        public TypeConverter(Type srcType, Type targetType)
        {
            this.srcType = srcType ?? throw new ArgumentNullException(nameof(srcType));
            this.targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        }

        public virtual Type Convert()
        {
            if (srcType == typeof(object))
            {
                return targetType;
            }

            return srcType;
        }
    }
}

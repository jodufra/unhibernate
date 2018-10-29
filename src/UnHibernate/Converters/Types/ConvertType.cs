using System;

namespace UnHibernate.Converters.Types
{
    internal class ConvertType
    {
        protected Type srcType;
        protected Type targetType;

        public ConvertType(Type srcType, Type targetType)
        {
            this.srcType = srcType ?? throw new ArgumentNullException(nameof(srcType));
            this.targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        }

        public virtual Type Execute()
        {
            if (srcType == typeof(object))
            {
                return targetType;
            }

            return srcType;
        }
    }
}

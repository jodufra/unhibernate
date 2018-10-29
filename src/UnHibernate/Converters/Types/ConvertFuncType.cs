using System;

namespace UnHibernate.Converters.Types
{
    internal class ConvertFuncType : ConvertType
    {
        public ConvertFuncType(Type srcType, Type targetType) : base(srcType, targetType)
        {
            if (!srcType.IsFuncType())
            {
                throw new ArgumentOutOfRangeException(nameof(srcType));
            }
        }

        public override Type Execute()
        {
            throw new NotImplementedException();
        }
    }
}

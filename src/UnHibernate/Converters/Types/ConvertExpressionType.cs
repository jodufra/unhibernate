using System;

namespace UnHibernate.Converters.Types
{
    internal class ConvertExpressionType : ConvertType
    {
        public ConvertExpressionType(Type srcType, Type targetType) : base(srcType, targetType)
        {
            if (!srcType.IsExpressionType())
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

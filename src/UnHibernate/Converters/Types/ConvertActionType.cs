using System;

namespace UnHibernate.Converters.Types
{
    internal class ConvertActionType : ConvertType
    {
        public ConvertActionType(Type srcType, Type targetType) : base(srcType, targetType)
        {
            if (!srcType.IsActionType())
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

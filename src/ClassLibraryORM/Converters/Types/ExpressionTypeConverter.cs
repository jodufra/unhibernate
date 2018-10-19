using System;

namespace ClassLibraryORM.Converters.Types
{
    internal class ExpressionTypeConverter : TypeConverter
    {
        public ExpressionTypeConverter(Type srcType, Type targetType) : base(srcType, targetType)
        {
            if (!srcType.IsExpression())
            {
                throw new ArgumentOutOfRangeException(nameof(srcType));
            }
        }

        public override Type Convert()
        {
            throw new NotImplementedException();
        }
    }
}

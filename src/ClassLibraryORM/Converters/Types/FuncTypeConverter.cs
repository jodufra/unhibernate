using System;

namespace ClassLibraryORM.Converters.Types
{
    internal class FuncTypeConverter : TypeConverter
    {
        public FuncTypeConverter(Type srcType, Type targetType) : base(srcType, targetType)
        {
            if (!srcType.IsFuncType())
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

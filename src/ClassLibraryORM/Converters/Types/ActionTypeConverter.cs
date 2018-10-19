using System;

namespace ClassLibraryORM.Converters.Types
{
    internal class ActionTypeConverter : TypeConverter
    {
        public ActionTypeConverter(Type srcType, Type targetType) : base(srcType, targetType)
        {
            if (!srcType.IsAction())
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

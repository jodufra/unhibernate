using System;

namespace ClassLibraryORM.Converters.Objects
{
    internal class DelegateObjectConverter : ObjectConverter
    {
        public DelegateObjectConverter(ObjectType objectType, Type targetType) : base(objectType, targetType)
        {
        }

        public override ObjectType Convert()
        {
            if (!(objectType.Obj is Delegate delegat))
            {
                return base.Convert();
            }


            throw new NotImplementedException();
        }
    }
}

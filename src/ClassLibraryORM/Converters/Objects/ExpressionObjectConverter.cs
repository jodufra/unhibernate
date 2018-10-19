using System;
using System.Linq.Expressions;

namespace ClassLibraryORM.Converters.Objects
{
    internal class ExpressionObjectConverter : ObjectConverter
    {
        public ExpressionObjectConverter(ObjectType objectType, Type targetType) : base(objectType, targetType)
        {
            if (!objectType.Type.IsExpression())
            {
                throw new ArgumentOutOfRangeException(nameof(objectType));
            }
        }

        public override ObjectType Convert()
        {
            var expression = objectType.Obj as Expression;


            // TODO
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Linq;
using ClassLibraryORM.Converters.Objects;
using ClassLibraryORM.Converters.Types;

namespace ClassLibraryORM.Converters
{
    internal static class ObjectTypeConverter
    {
        public static ObjectType Convert(ObjectType objectType, Type entityType)
        {
            var type = objectType.Type;

            if (type.GenericTypeArguments.Any())
            {
                if (type.IsExpression())
                {
                    return new ExpressionObjectConverter(objectType, entityType).Convert();
                }
                else if (type.IsFunc())
                {
                    return new FuncObjectConverter(objectType, entityType).Convert();
                }
                else if (type.IsAction())
                {
                    return new ActionObjectConverter(objectType, entityType).Convert();
                }
            }
            else if (type == typeof(object))
            {
                return new ObjectConverter(objectType, entityType).Convert();
            }

            // no operation
            return objectType;
        }

        public static Type Convert(Type type, Type entityType)
        {
            if (type.GenericTypeArguments.Any())
            {
                if (type.IsExpression())
                {
                    return new ExpressionTypeConverter(type, entityType).Convert();
                }
                else if (type.IsFunc())
                {
                    return new FuncTypeConverter(type, entityType).Convert();
                }
                else if (type.IsAction())
                {
                    return new ActionTypeConverter(type, entityType).Convert();
                }
            }
            else if(type == typeof(object))
            {
                return new TypeConverter(type, entityType).Convert();
            }

            // no operation
            return type;
        }
    }
}

using ClassLibraryORM.Converters.Objects;
using ClassLibraryORM.Converters.Types;
using System;
using System.Linq;

namespace ClassLibraryORM.Converters
{
    internal static class ObjectTypeConverter
    {
        public static ObjectType Convert(ObjectType objectType, Type targetType)
        {
            var type = objectType.Type;
            if (type == targetType)
            {
                return objectType;
            }

            if (type.GenericTypeArguments.Any())
            {
                if (type.IsExpression())
                {
                    return new ExpressionObjectConverter(objectType, targetType).Convert();
                }
                else if (type.IsFunc())
                {
                    return new FuncObjectConverter(objectType, targetType).Convert();
                }
                else if (type.IsAction())
                {
                    return new ActionObjectConverter(objectType, targetType).Convert();
                }
            }
            else if (type == typeof(object))
            {
                return new ObjectConverter(objectType, targetType).Convert();
            }

            // no operation
            return objectType;
        }

        public static Type Convert(Type type, Type targetType)
        {
            if (type == targetType)
            {
                return targetType;
            }

            if (type.GenericTypeArguments.Any())
            {
                if (type.IsExpression())
                {
                    return new ExpressionTypeConverter(type, targetType).Convert();
                }
                else if (type.IsFunc())
                {
                    return new FuncTypeConverter(type, targetType).Convert();
                }
                else if (type.IsAction())
                {
                    return new ActionTypeConverter(type, targetType).Convert();
                }
            }
            else if (type == typeof(object))
            {
                return new TypeConverter(type, targetType).Convert();
            }

            // no operation
            return type;
        }
    }
}

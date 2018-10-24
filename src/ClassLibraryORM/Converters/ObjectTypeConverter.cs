using ClassLibraryORM.Converters.Objects;
using ClassLibraryORM.Converters.Types;
using System;

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

            if (type.IsExpressionType())
            {
                return new ExpressionObjectConverter(objectType, targetType).Convert();
            }

            if (type.IsFuncType())
            {
                return new FuncObjectConverter(objectType, targetType).Convert();
            }

            if (type.IsActionType())
            {
                return new ActionObjectConverter(objectType, targetType).Convert();
            }

            if (type.IsDelegateType())
            {
                return new DelegateObjectConverter(objectType, targetType).Convert();
            }

            if (type == typeof(object))
            {
                return new ObjectConverter(objectType, targetType).Convert();
            }
            
            return objectType;
        }

        public static Type Convert(Type type, Type targetType)
        {
            if (type == targetType)
            {
                return targetType;
            }

            if (type.IsExpressionType())
            {
                return new ExpressionTypeConverter(type, targetType).Convert();
            }

            if (type.IsFuncType())
            {
                return new FuncTypeConverter(type, targetType).Convert();
            }

            if (type.IsActionType())
            {
                return new ActionTypeConverter(type, targetType).Convert();
            }

            if (type == typeof(object))
            {
                return new TypeConverter(type, targetType).Convert();
            }
            
            return type;
        }
    }
}

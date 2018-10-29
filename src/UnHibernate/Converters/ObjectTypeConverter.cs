using UnHibernate.Converters.Objects;
using UnHibernate.Converters.Types;
using System;

namespace UnHibernate.Converters
{
    internal static class ObjectTypeConverter
    {
        public static ObjectType Convert(object instance, ObjectType objectType, Type targetType)
        {
            var type = objectType.Type;

            if (type == targetType)
            {
                return objectType;
            }

            if (type.IsExpressionType())
            {
                return new ConvertExpressionObject(instance, objectType, targetType).Execute();
            }

            if (type.IsFuncType())
            {
                return new ConvertFuncObject(instance, objectType, targetType).Execute();
            }

            if (type.IsActionType())
            {
                return new ConvertActionObject(instance, objectType, targetType).Execute();
            }

            if (type.IsDelegateType())
            {
                return new ConvertDelegateObject(instance, objectType, targetType).Execute();
            }

            if (type == typeof(object))
            {
                return new ConvertObject(instance, objectType, targetType).Execute();
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
                return new ConvertExpressionType(type, targetType).Execute();
            }

            if (type.IsFuncType())
            {
                return new ConvertFuncType(type, targetType).Execute();
            }

            if (type.IsActionType())
            {
                return new ConvertActionType(type, targetType).Execute();
            }

            if (type == typeof(object))
            {
                return new ConvertType(type, targetType).Execute();
            }
            
            return type;
        }
    }
}

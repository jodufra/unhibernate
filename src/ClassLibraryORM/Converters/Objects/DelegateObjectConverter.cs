using System;
using System.Linq;
using System.Reflection;

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

            var parameters = delegat.Method.GetParameters();
            var returnParameter = delegat.Method.ReturnParameter;

            if (returnParameter.ParameterType != typeof(object) && !parameters.Any(q => q.ParameterType == typeof(object)))
            {
                // return original ojectType
                return objectType;
            }

            throw new NotImplementedException();
        }

        private ParameterInfo[] ConvertParameters(ParameterInfo[] parameters)
        {
            return parameters;
        }
    }
}

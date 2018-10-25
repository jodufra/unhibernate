﻿using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ClassLibraryORM.Converters.Objects
{
    internal class DelegateObjectConverter : ObjectConverter
    {
        public DelegateObjectConverter(object instance, ObjectType objectType, Type targetType) : base(instance, objectType, targetType)
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

            if (!RequiresConversion(parameters, returnParameter))
            {
                // return original ojectType
                return objectType;
            }

            var domain = AppDomain.CurrentDomain;
            var asmName = delegat.Method.DeclaringType.Assembly.GetName();
            var asmBuilder = domain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
            var module = asmBuilder.DefineDynamicModule(asmName.Name, $"{asmName.Name}.dll");


            throw new NotImplementedException();
        }

        private ParameterInfo[] ConvertParameters(ParameterInfo[] parameters)
        {
            return parameters;
        }

        private static bool RequiresConversion(ParameterInfo[] argumentParameters, ParameterInfo returnParameter)
        {
            return !RequiresConversion(returnParameter.ParameterType) && !argumentParameters.Any(q => RequiresConversion(q.ParameterType));
        }

        private static bool RequiresConversion(Type type)
        {
            return type == typeof(object) || type.GenericTypeArguments.Any(RequiresConversion);
        }
    }
}

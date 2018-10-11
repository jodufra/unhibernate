using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Mapping.ByCode;

namespace ClassLibraryORM.Converters
{
    internal abstract class Converter
    {
        protected ArgumentType argumentType;
        protected Type entityType;

        protected Converter(ArgumentType argumentType, Type entityType)
        {
            this.argumentType = argumentType;
            this.entityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        }

        protected abstract ArgumentType Apply();

        public static ArgumentType Convert(ArgumentType argumentType, Type entityType)
        {
            var type = argumentType.Type;

            if (type.IsExpression() && type.GenericTypeArguments.Any())
            {
                return new ExpressionConverter(argumentType, entityType).Apply();
            }
            else if (type.IsFunc() && type.GenericTypeArguments.Any())
            {
                return new FuncConverter(argumentType, entityType).Apply();
            }
            else if (type.IsAction() && type.GenericTypeArguments.Any())
            {
                return new ActionConverter(argumentType, entityType).Apply();
            }

            // no op

            return argumentType;
        }
    }
}

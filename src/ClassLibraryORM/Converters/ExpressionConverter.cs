using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryORM.Converters
{
    internal class ExpressionConverter : Converter
    {
        public ExpressionConverter(ArgumentType argumentType, Type entityType) : base(argumentType, entityType)
        {
            if (!argumentType.Type.IsExpression())
                throw new ArgumentOutOfRangeException(nameof(argumentType));
        }

        protected override ArgumentType Apply()
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}

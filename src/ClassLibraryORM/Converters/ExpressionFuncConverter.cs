using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryORM.Converters
{
    internal class ExpressionFuncConverter : Converter
    {
        protected override ArgumentType ApplyConversion(ArgumentType argumentType, Type entityType)
        {
            argumentType = base.ApplyConversion(argumentType, entityType);

            throw new NotImplementedException();
        }
    }
}

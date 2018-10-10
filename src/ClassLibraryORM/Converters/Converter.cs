using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryORM.Converters
{
    internal abstract class Converter
    {
        protected virtual ArgumentType ApplyConversion(ArgumentType argumentType, Type entityType)
        {
            if (argumentType.Arg == null || argumentType.Type == null)
            {
                throw new ArgumentException(nameof(argumentType));
            }
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            return argumentType;
        }

        public static ArgumentType Convert(ArgumentType argumentType, Type entityType)
        {
            var type = argumentType.Type;
            if(type == null)
            {

            }
            return argumentType;
        }
    }
}

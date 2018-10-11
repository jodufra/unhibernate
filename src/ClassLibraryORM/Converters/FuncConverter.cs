using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryORM.Converters
{
    internal class FuncConverter : Converter
    {
        public FuncConverter(ArgumentType argumentType, Type entityType) : base(argumentType, entityType)
        {
            if (!argumentType.Type.IsFunc())
                throw new ArgumentOutOfRangeException(nameof(argumentType));
        }

        protected override ArgumentType Apply()
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}

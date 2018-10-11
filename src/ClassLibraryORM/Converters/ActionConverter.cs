using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryORM.Converters
{
    internal class ActionConverter : Converter
    {
        public ActionConverter(ArgumentType argumentType, Type entityType) : base(argumentType, entityType)
        {
            if (!argumentType.Type.IsAction())
                throw new ArgumentOutOfRangeException(nameof(argumentType));
        }

        protected override ArgumentType Apply()
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}

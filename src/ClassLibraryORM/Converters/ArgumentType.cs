using System;

namespace ClassLibraryORM.Converters
{
    internal struct ArgumentType
    {
        public ArgumentType(object arg) : this()
        {
            Arg = arg ?? throw new ArgumentNullException(nameof(arg));
            Type = arg.GetType();
        }

        public object Arg { get; set; }
        public Type Type { get; set; }
    }
}

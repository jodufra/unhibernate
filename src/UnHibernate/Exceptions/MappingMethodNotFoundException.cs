using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;
using System.Text;

namespace UnHibernate.Exceptions
{
    public class MappingMethodNotFoundException : Exception
    {
        protected MappingMethodNotFoundException()
        {
        }

        protected MappingMethodNotFoundException(string message) : base(message)
        {
        }

        protected MappingMethodNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MappingMethodNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MappingMethodNotFoundException(InvokeMemberBinder binder, Type classMappingType, Type entityType, IEnumerable<Type> processedTypes) : 
            base(FormatMessage(binder, classMappingType, entityType, processedTypes))
        {
        }

        private static string FormatMessage(InvokeMemberBinder binder, Type classMappingType, Type entityType, IEnumerable<Type> processedTypes)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Unable to find method '{binder.Name}({string.Join(", ", binder.CallInfo.ArgumentNames)})' in '{classMappingType.Name}<{entityType.Name}>' accepting the following argument types:");
            foreach (var type in processedTypes)
            {
                sb.AppendLine(type.FullName);
            }

            return sb.ToString();
        }
    }
}

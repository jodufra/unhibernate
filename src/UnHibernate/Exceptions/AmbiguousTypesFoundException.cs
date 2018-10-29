using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnHibernate.Exceptions
{
    class AmbiguousTypesFoundException : Exception
    {
        protected AmbiguousTypesFoundException()
        {
        }

        protected AmbiguousTypesFoundException(string message) : base(message)
        {
        }

        protected AmbiguousTypesFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AmbiguousTypesFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AmbiguousTypesFoundException(IEnumerable<Type> ambiguousTypes)
            : base(FormatMessage(ambiguousTypes))
        {

        }

        private static string FormatMessage(IEnumerable<Type> assignableTypes)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Found multiple deep assignable types with the same 'MemberInfo.Name':");
            foreach (var t in assignableTypes)
            {
                sb.AppendLine(t.FullName);
            }

            return sb.ToString();
        }
    }
}

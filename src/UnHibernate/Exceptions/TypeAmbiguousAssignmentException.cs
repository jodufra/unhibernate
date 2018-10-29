using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnHibernate.Exceptions
{
    public class TypeAmbiguousAssignmentException : Exception
    {
        protected TypeAmbiguousAssignmentException()
        {
        }

        protected TypeAmbiguousAssignmentException(string message) : base(message)
        {
        }

        protected TypeAmbiguousAssignmentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TypeAmbiguousAssignmentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public TypeAmbiguousAssignmentException(Type type, IEnumerable<Type> assignableTypes)
            : base(FormatMessage(type, assignableTypes))
        {

        }

        private static string FormatMessage(Type type, IEnumerable<Type> assignableTypes)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Type '{type.FullName}' must not be deep assignable from multiple types.");
            sb.AppendLine($"Ensure the type can't be assignable from multiple types or ensure all those multiple types are assignable from a single type.");
            sb.AppendLine($"Deep assignable types:");
            foreach (var at in assignableTypes)
            {
                sb.AppendLine(at.FullName);
            }

            return sb.ToString();
        }
    }
}

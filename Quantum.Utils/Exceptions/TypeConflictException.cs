using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Utils
{
    public class TypeConflictException : Exception
    {
        public IEnumerable<Type> ConflictedTypes { get; private set; }

        public TypeConflictException(Type type)
            : this(Enumerable.Repeat(type, 1))
        {
        }

        public TypeConflictException(Type type, string message)
            : this(Enumerable.Repeat(type, 1), message)
        {
        }

        public TypeConflictException(Type type, string message, Exception innerException)
            : this(Enumerable.Repeat(type, 1), message, innerException)
        {
        }

        public TypeConflictException(IEnumerable<Type> types)
        {
            ConflictedTypes = types;
        }

        public TypeConflictException(IEnumerable<Type> types, string message)
            : base(message)
        {
            ConflictedTypes = types;
        }

        public TypeConflictException(IEnumerable<Type> types, string message, Exception innerException)
            : base(message, innerException)
        {
            ConflictedTypes = types;
        }
    }
}

using System;

namespace Quantum.Utils
{
    public class TypeNotFoundException : Exception
    {
        public TypeNotFoundException()
        {
        }
        
        public TypeNotFoundException(string message)
            : base(message)
        {
        }

        public TypeNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

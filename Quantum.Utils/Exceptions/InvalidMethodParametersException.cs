using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Exceptions
{
    public class InvalidMethodParametersException : Exception
    {
        public IEnumerable<Type> ExpectedParameterTypes { get; private set; }

        public InvalidMethodParametersException()
        {
        }

        public InvalidMethodParametersException(string message)
            : base(message)
        {
        }

        public InvalidMethodParametersException(params Type[] expectedParameterTypes)
        {
            ExpectedParameterTypes = expectedParameterTypes;
        }

        public InvalidMethodParametersException(string message, params Type[] expectedParameterTypes)
            : base(message)
        {
            ExpectedParameterTypes = expectedParameterTypes;
        }
    }
}

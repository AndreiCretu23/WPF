using System;
using System.Diagnostics;
using System.Reflection;

namespace Quantum.Exceptions
{
    public class MissingAttributeException : Exception
    {
        public MemberInfo Member { get; private set; }
        public Type ExpectedAttributeType { get; private set; }

        public MissingAttributeException(MemberInfo memberInfo, Type attributeType)
        {
            Debug.Assert(typeof(Attribute).IsAssignableFrom(attributeType));
            Member = memberInfo;
            ExpectedAttributeType = attributeType;
        }

        public MissingAttributeException(MemberInfo memberInfo, Type attributeType, string message)
            : base(message)
        {
            Debug.Assert(typeof(Attribute).IsAssignableFrom(attributeType));
            Member = memberInfo;
            ExpectedAttributeType = attributeType;
        }
    }
}

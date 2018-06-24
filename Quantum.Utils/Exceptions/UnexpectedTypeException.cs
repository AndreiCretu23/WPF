﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Exceptions
{
    public class UnexpectedTypeException : Exception
    {
        public Type ExpectedType { get; private set; }
        public Type ActualType { get; private set; }

        public UnexpectedTypeException(Type expectedType, Type actualType)
            : base($"Error : Unexpected Type. \nExpected Type was : {expectedType.Name} \nActual Type is : {actualType.Name}")
        {
            ExpectedType = expectedType;
            ActualType = actualType;
        }

        public UnexpectedTypeException(Type expectedType, Type actualType, string message)
            : base (message)
        {
            ExpectedType = expectedType;
            ActualType = actualType;
        }

        public UnexpectedTypeException(Type expectedType, Type actualType, string message, Exception innerException)
            : base(message, innerException)
        {
            ExpectedType = expectedType;
            ActualType = actualType;
        }
    }
}

﻿using System;

namespace Quantum.Exceptions
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

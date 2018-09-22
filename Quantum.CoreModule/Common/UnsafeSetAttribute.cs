using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.CoreModule
{
    /// <summary>
    /// Properties that are decorated with this attribute are not safe to set. 
    /// Setting them might cause side-effects/uncontrolled exceptions. 
    /// </summary>
    [AttributeUsage(validOn:AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UnsafeSetAttribute : Attribute
    {
        public string Reason { get; }

        public UnsafeSetAttribute() { }
        public UnsafeSetAttribute(string reason) { Reason = reason; }

    }
}

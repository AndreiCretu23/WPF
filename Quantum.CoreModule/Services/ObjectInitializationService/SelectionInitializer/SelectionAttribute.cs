using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Services
{
    /// <summary>
    /// In objects initialized by the IObjectInitializationService, all selection properties
    /// decorated with this attribute will automatically get resolved from the container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class SelectionAttribute : Attribute
    {
    }
}

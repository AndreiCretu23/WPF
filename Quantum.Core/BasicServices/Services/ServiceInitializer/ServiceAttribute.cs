using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
    /// <summary>
    /// In objects initialized by the IObjectInitializationService, fields and properties decorated with this attribute will automatically get
    /// resolved. It's the equivalent of calling 
    /// <code>
    /// Field/Property = Container.Resolve()
    /// </code>
    /// in the constructor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class ServiceAttribute : Attribute
    {
    }
}

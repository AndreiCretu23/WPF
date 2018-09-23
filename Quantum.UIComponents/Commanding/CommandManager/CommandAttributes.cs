using System;

namespace Quantum.Command
{
    /// <summary>
    /// Signals the CommandManagerService to ignore a particular command property when registering the parent command container.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IgnoreCommandAttribute : Attribute
    {
    }
}

using Quantum.Command;
using System;

namespace Quantum.UIComponents
{
    /// <summary>
    /// In objects initialized by the IObjectInitializeService, properties decorated with this attribute will be assigned
    /// the command in the specified command container which matches the property type and name. If, in the specified command container, 
    /// no command is found which matches the property definition(type/name), or the specified command container is not registered in the command manager, 
    /// an exception will be thrown.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CommandAttribute : Attribute
    {
        public Type CommandContainerType { get; }

        public CommandAttribute(Type commandContainerType)
        {
            CommandContainerType = commandContainerType;
        }
        
    }
}

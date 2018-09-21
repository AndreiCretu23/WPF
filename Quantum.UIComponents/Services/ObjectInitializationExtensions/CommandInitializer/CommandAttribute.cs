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
            AssertCommandContainerType(commandContainerType);
            CommandContainerType = commandContainerType;
        }

        private void AssertCommandContainerType(Type commandContainerType)
        {
            if(commandContainerType == null)
            {
                throw new Exception($"Error : {typeof(CommandAttribute).Name} : Null value for parameter {nameof(commandContainerType)} is not allowed.");
            }

            if(!typeof(ICommandContainer).IsAssignableFrom(commandContainerType))
            {
                throw new Exception($"Error : {typeof(CommandAttribute).Name} : Parameter {nameof(commandContainerType)} must be the type of a CommandContainer (A class or interface that implements ICommanContainer that has been registered in the CommnadManagerService).");
            }
        }

    }
}

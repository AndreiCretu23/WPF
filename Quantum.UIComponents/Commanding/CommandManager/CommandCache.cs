using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.Command
{
    internal class CommandCache
    {
        private IList<CommandCacheEntry> CachedCommands { get; set; } = new List<CommandCacheEntry>();
        
        internal void AddCommand(object command, Type commandContainer, string commandName)
        {
            CachedCommands.Add(new CommandCacheEntry()
            {
                Command = command, 
                CommandContainer = commandContainer, 
                CommandName = commandName,
            });
        }

        internal IEnumerable<object> GetCommands()
        {
            return CachedCommands.Select(o => o.Command);
        }

        internal IEnumerable<TCommand> GetCommandsOfType<TCommand>()
        {
            return GetCommands().OfType<TCommand>();
        }

        internal object GetCommand(Type commandContainer, string commandName)
        {
            try
            {
                return CachedCommands.Single(o => commandContainer.IsAssignableFrom(o.CommandContainer) && commandName == o.CommandName);
            }
            catch(InvalidOperationException)
            {
                throw new Exception($"The requested command : {commandContainer.Name} : {commandName} has not been registered");
            }
        }

        internal TCommand GetCommand<TCommand>(Type commandContainer, string commandName)
        {
            try
            {
                return CachedCommands.Single(o => commandContainer.IsAssignableFrom(o.CommandContainer) &&
                                              o.CommandName == commandName &&
                                              typeof(TCommand).IsAssignableFrom(o.Command.GetType()))
                                 .Command.SafeCast<TCommand>();
            }
            catch(InvalidOperationException)
            {
                throw new Exception($"The requested command : {commandContainer.Name} : {typeof(TCommand).Name} {commandName} has not been registered.");
            }
        }

        internal string GetCommandName(object command)
        {
            return CachedCommands.Single(c => c.Command == command).CommandName;
        }
        
        internal IEnumerable<Type> GetRegisteredContainers()
        {
            return CachedCommands.Select(o => o.CommandContainer).Distinct();
        }

        internal bool IsRegistered(object command)
        {
            return CachedCommands.Any(c => c.Command == command);
        }
    }

    internal class CommandCacheEntry
    {
        internal object Command { get; set; }
        internal Type CommandContainer { get; set; }
        internal string CommandName { get; set; }
    }
}

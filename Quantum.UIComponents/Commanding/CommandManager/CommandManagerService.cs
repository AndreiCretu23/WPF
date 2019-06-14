using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Quantum.Command
{
    internal class CommandManagerService : ServiceBase, ICommandManagerService
    {
        [Service]
        public IMetadataAsserterService MetadataAsserter { get; set; }

        [Service]
        public ICommandInvalidationManagerService InvalidationManager { get; set; }
        
        private CommandCache CachedCommands { get; set; } = new CommandCache();
        public IEnumerable<object> Commands { get => CachedCommands.GetCommands(); }
        public IEnumerable<IGlobalCommand> GlobalCommands { get => CachedCommands.GetCommandsOfType<IGlobalCommand>(); }
        public IEnumerable<IMultiGlobalCommand> MultiGlobalCommands { get => CachedCommands.GetCommandsOfType<IMultiGlobalCommand>(); }
        
        public CommandManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        
        public void RegisterCommandContainer<IContainer, TContainer>()
            where TContainer : class, IContainer
            where IContainer : class, ICommandContainer
        {
            if(CachedCommands.GetRegisteredContainers().Contains(typeof(TContainer)))
            {
                throw new Exception($"Error : The Command Container {typeof(TContainer).Name} has already been registered.");
            }

            Container.RegisterService<IContainer, TContainer>();
            AddCommandContainer(Container.Resolve<IContainer>());
        }

        private void AddCommandContainer<IContainer>(IContainer commandContainer)
            where IContainer : class, ICommandContainer
        {
            var containerType = commandContainer.GetType();
            var properties = containerType.GetProperties().Where(prop => !prop.HasAttribute<IgnoreCommandAttribute>());

            var containerGlobalCommands = new Collection<IGlobalCommand>();
            var containerMultiGlobalCommands = new Collection<IMultiGlobalCommand>();

            var commandNames = new Dictionary<object, string>();
            
            foreach(var prop in properties)
            {
                if(typeof(IGlobalCommand).IsAssignableFrom(prop.PropertyType))
                {
                    var command = (IGlobalCommand)prop.GetValue(commandContainer);
                    AssertCommandPropertyNotNull(command, containerType, prop.Name);
                    containerGlobalCommands.Add(command);
                    commandNames.Add(command, prop.Name);   
                }
                
                else if(typeof(IMultiGlobalCommand).IsAssignableFrom(prop.PropertyType))
                {
                    var command = (IMultiGlobalCommand)prop.GetValue(commandContainer);
                    AssertCommandPropertyNotNull(command, containerType, prop.Name);
                    containerMultiGlobalCommands.Add(command);
                    commandNames.Add(command, prop.Name);
                }
            }
            
            containerGlobalCommands.ForEach(c =>
            {
                MetadataAsserter.AssertMetadataCollectionProperties(c, commandNames[c]);
                InvalidationManager.ProcessInvalidators(c);
            });

            containerMultiGlobalCommands.ForEach(c =>
            {
                MetadataAsserter.AssertMetadataCollectionProperties(c, commandNames[c]);
                c.OnCommandsComputed += (oldCommands, newCommands) =>
                {
                    newCommands.ForEach(_ =>
                    {
                        MetadataAsserter.AssertMetadataCollectionProperties(_, $"{commandNames[c]} -> ComputedCommands");
                    });
                };
                InvalidationManager.ProcessInvalidators(c);
            });
            
            containerGlobalCommands.ForEach(c => CachedCommands.AddCommand(c, containerType, commandNames[c]));
            containerMultiGlobalCommands.ForEach(c => CachedCommands.AddCommand(c, containerType, commandNames[c]));
        }

        [DebuggerHidden]
        private void AssertCommandPropertyNotNull(object command, Type commandContainer, string commandPropertyName)
        {
            if (command == null)
            {
                throw new Exception($"Error registering the command {commandContainer.Name}.{commandPropertyName}. Property value is null!");
            }
        }



        public object GetCommand<TCommandContainer>(Expression<Func<TCommandContainer, object>> commandProperty)
            where TCommandContainer : ICommandContainer
        {
            commandProperty.AssertParameterNotNull(nameof(commandProperty));
            return CachedCommands.GetCommand(typeof(TCommandContainer), ReflectionUtils.GetPropertyName(commandProperty));
        }

        public TCommand GetCommand<TCommandContainer, TCommand>(Expression<Func<TCommandContainer, TCommand>> commandProperty)
            where TCommandContainer : ICommandContainer
        {
            commandProperty.AssertParameterNotNull(nameof(commandProperty));
            return CachedCommands.GetCommand<TCommand>(typeof(TCommandContainer), ReflectionUtils.GetPropertyName(commandProperty));
        }
        
        public string GetCommandName(object command)
        {
            command.AssertParameterNotNull(nameof(command));
            try 
            {
                return CachedCommands.GetCommandName(command);
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"Error retrieving the name of the requested command : The command is not registered in the CommandManager.");
            }
        }
        
        public bool IsRegistered(object command)
        {
            command.AssertParameterNotNull(nameof(command));
            return CachedCommands.IsRegistered(command);
        }

    }
}

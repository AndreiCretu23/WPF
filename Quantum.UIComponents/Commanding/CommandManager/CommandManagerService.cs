using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Quantum.Command
{
    internal class CommandManagerService : QuantumServiceBase, ICommandManagerService
    {
        [Service]
        public IMetadataAsserterService MetadataAsserter { get; set; }

        [Service]
        public ICommandMetadataProcessorService CommandMetadataProcessor { get; set; }

        
        private CommandCache CachedCommands { get; set; } = new CommandCache();
        public IEnumerable<object> Commands { get => CachedCommands.GetCommands(); }
        public IEnumerable<IManagedCommand> ManagedCommands { get => CachedCommands.GetCommandsOfType<IManagedCommand>(); }
        public IEnumerable<IMultiManagedCommand> MultiManagedCommands { get => CachedCommands.GetCommandsOfType<IMultiManagedCommand>(); }
        
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

            var containerManagedCommands = new Collection<IManagedCommand>();
            var containerMultiManagedCommands = new Collection<IMultiManagedCommand>();

            var commandNames = new Dictionary<object, string>();
            
            foreach(var prop in properties)
            {
                if(typeof(IManagedCommand).IsAssignableFrom(prop.PropertyType))
                {
                    var command = (IManagedCommand)prop.GetValue(commandContainer);
                    containerManagedCommands.Add(command);
                    commandNames.Add(command, prop.Name);   
                }
                
                else if(typeof(IMultiManagedCommand).IsAssignableFrom(prop.PropertyType))
                {
                    var command = (IMultiManagedCommand)prop.GetValue(commandContainer);
                    containerMultiManagedCommands.Add(command);
                    commandNames.Add(command, prop.Name);
                }
            }
            
            containerManagedCommands.ForEach(c =>
            {
                MetadataAsserter.AssertMetadataCollectionProperties(c, commandNames[c]);
                CommandMetadataProcessor.ProcessMetadata(c, cmd => cmd.Metadata);
            });

            containerMultiManagedCommands.ForEach(c =>
            {
                MetadataAsserter.AssertMetadataCollectionProperties(c, commandNames[c]);
                c.OnCommandsComputed += computedCommands =>
                {
                    computedCommands.ForEach(_ =>
                    {
                        MetadataAsserter.AssertMetadataCollectionProperties(_, $"{commandNames[c]} -> ComputedCommands");
                        CommandMetadataProcessor.ProcessMetadata(_, subCmd => subCmd.Metadata);
                    });
                };
            });
            
            containerManagedCommands.ForEach(c => CachedCommands.AddCommand(c, containerType, commandNames[c]));
            containerMultiManagedCommands.ForEach(c => CachedCommands.AddCommand(c, containerType, commandNames[c]));
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
        
    }
}

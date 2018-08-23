using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quantum.Command
{
    public interface ICommandManagerService
    {
        IEnumerable<IManagedCommand> ManagedCommands { get; }
        IEnumerable<IMultiManagedCommand> MultiManagedCommands { get; }
        
        void RegisterCommandContainer<TCommandContainer>() where TCommandContainer : class, ICommandContainer;
        void RegisterCommandContainer<IContainer, TContainer>() where TContainer : class, IContainer 
                                                                where IContainer : class, ICommandContainer;
    }

    internal class CommandManagerService : QuantumServiceBase, ICommandManagerService
    {
        [Service]
        public IMetadataAsserterService MetadataAsserter { get; set; }

        [Service]
        public ICommandMetadataProcessorService CommandMetadataProcessor { get; set; }
        
        #region InternalLists

        private List<IManagedCommand> managedCommands { get; set; } = new List<IManagedCommand>();
        private List<IMultiManagedCommand> multiManagedCommands { get; set; } = new List<IMultiManagedCommand>();

        #endregion InternalLists

        public IEnumerable<IManagedCommand> ManagedCommands { get => managedCommands; }
        public IEnumerable<IMultiManagedCommand> MultiManagedCommands { get => multiManagedCommands; }
        
        public CommandManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public void RegisterCommandContainer<TCommandContainer>()
            where TCommandContainer : class, ICommandContainer
        {
            Container.RegisterService<TCommandContainer>();
            AddCommandContainer(Container.Resolve<TCommandContainer>());
        }

        public void RegisterCommandContainer<IContainer, TContainer>()
            where TContainer : class, IContainer
            where IContainer : class, ICommandContainer
        {
            Container.RegisterService<IContainer, TContainer>();
            AddCommandContainer(Container.Resolve<IContainer>());
        }

        private void AddCommandContainer<TCommandContainer>(TCommandContainer commandContainer)
            where TCommandContainer : class, ICommandContainer
        {
            var properties = typeof(TCommandContainer).GetProperties();

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

            managedCommands.AddRange(containerManagedCommands);
            multiManagedCommands.AddRange(containerMultiManagedCommands);
        }
        
    }
}

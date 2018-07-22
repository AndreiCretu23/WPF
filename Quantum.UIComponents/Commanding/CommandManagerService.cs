using Quantum.Services;
using Quantum.UIComponents;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using Quantum.Metadata;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Command
{
    public interface ICommandManagerService
    {
        IEnumerable<ManagedCommand> ManagedCommands { get; }
        IEnumerable<MultiManagedCommand> MultiManagedCommands { get; }
        
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
        private List<ManagedCommand> managedCommands { get; set; } = new List<ManagedCommand>();
        private List<MultiManagedCommand> multiManagedCommands { get; set; } = new List<MultiManagedCommand>();

        #endregion InternalLists

        public IEnumerable<ManagedCommand> ManagedCommands { get => managedCommands; }
        public IEnumerable<MultiManagedCommand> MultiManagedCommands { get => multiManagedCommands; }
        
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

            var containerManagedCommands = new List<ManagedCommand>();
            var containerMultiManagedCommands = new List<MultiManagedCommand>();

            var commandNames = new Dictionary<object, string>();
            
            foreach(var prop in properties)
            {
                if(prop.PropertyType == typeof(ManagedCommand))
                {
                    var command = (ManagedCommand)prop.GetValue(commandContainer);
                    containerManagedCommands.Add(command);
                    commandNames.Add(command, prop.Name);   
                }
                
                else if(prop.PropertyType == typeof(MultiManagedCommand))
                {
                    var command = (MultiManagedCommand)prop.GetValue(commandContainer);
                    containerMultiManagedCommands.Add(command);
                    commandNames.Add(command, prop.Name);
                }
            }
            
            containerManagedCommands.ForEach(c =>
            {
                MetadataAsserter.AssertMetadataCollections(c, commandNames[c]);
                CommandMetadataProcessor.ProcessMetadata(c);
            });

            containerMultiManagedCommands.ForEach(c =>
            {
                MetadataAsserter.AssertMetadataCollections(c, commandNames[c]);
            });

            managedCommands.AddRange(containerManagedCommands);
            multiManagedCommands.AddRange(containerMultiManagedCommands);
        }
        
    }
}

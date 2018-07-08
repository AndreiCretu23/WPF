using Quantum.Services;
using Quantum.UIComponents;
using Quantum.Utils;
using System;
using System.Collections.Generic;
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
            MetadataAsserter.AssertMetadata<TCommandContainer>();

            var properties = typeof(TCommandContainer).GetProperties();
            var containerManagedCommands = properties.Where(prop => prop.PropertyType == typeof(ManagedCommand)).Select(prop => (ManagedCommand)prop.GetValue(commandContainer));
            var containerMultiManagedCommands = properties.Where(prop => prop.PropertyType == typeof(MultiManagedCommand)).Select(prop => (MultiManagedCommand)prop.GetValue(commandContainer));

            containerManagedCommands.ForEach(c => CommandMetadataProcessor.ProcessMetadata(c));

            managedCommands.AddRange(containerManagedCommands);
            multiManagedCommands.AddRange(containerMultiManagedCommands);
        }
        
    }
}

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
        
        void RegisterCommandContainer<TCommandContainer>() where TCommandContainer : class, ICommandContainer;
        void RegisterCommandContainer<IContainer, TContainer>() where TContainer : class, IContainer 
                                                                where IContainer : class, ICommandContainer;
    }

    internal class CommandManagerService : QuantumServiceBase, ICommandManagerService
    {
        [Service]
        public ICommandMetadataProcessorService CommandMetadataProcessor { get; set; }

        #region InternalLists
        private List<ManagedCommand> managedCommands { get; set; } = new List<ManagedCommand>();
        
        #endregion InternalLists

        public IEnumerable<ManagedCommand> ManagedCommands { get => managedCommands; }
        
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
            var containerManagedCommands = properties.Where(prop => prop.PropertyType == typeof(ManagedCommand)).Select(prop => (ManagedCommand)prop.GetValue(commandContainer));

            containerManagedCommands.ForEach(c => CommandMetadataProcessor.ProcessMetadata(c));


            managedCommands.AddRange(containerManagedCommands);
        }
        
    }
}

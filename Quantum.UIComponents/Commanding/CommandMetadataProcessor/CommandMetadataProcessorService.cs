using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Exceptions;
using Quantum.Services;
using Quantum.Utils;
using Quantum.Metadata;
using System;
using System.Collections.Generic;

namespace Quantum.Command
{
    internal class CommandMetadataProcessorService : ServiceBase, ICommandMetadataProcessorService
    {
        public CommandMetadataProcessorService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        
        public void ProcessMetadata<TCommand, TCollection>(TCommand command, Func<TCommand, TCollection> getMetadataCollection)
            where TCommand : IUICommand
            where TCollection : IEnumerable<IMetadataDefinition>
        {
            foreach(var metadata in getMetadataCollection(command))
            {
                metadata.IfIs((AutoInvalidateOnEvent e) => EventAggregator.Subscribe(e.EventType, () => command.RaiseCanExecuteChanged(), ThreadOption.UIThread, true));
                metadata.IfIs((AutoInvalidateOnSelection s) => EventAggregator.Subscribe(s.SelectionType, () => command.RaiseCanExecuteChanged(), ThreadOption.UIThread, true));
            }
        }
    }
}

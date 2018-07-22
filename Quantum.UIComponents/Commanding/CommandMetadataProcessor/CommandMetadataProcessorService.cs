using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Exceptions;
using Quantum.Services;
using Quantum.Utils;
using Quantum.Metadata;
using System;

namespace Quantum.Command
{
    internal class CommandMetadataProcessorService : QuantumServiceBase, ICommandMetadataProcessorService
    {
        public CommandMetadataProcessorService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        
        public void ProcessMetadata(ICommandBase command)
        {
            foreach (var metadata in command.CommandMetadata) {
                if (metadata == null) continue;

                metadata.IfIs((AutoInvalidateOnEvent e) => EventAggregator.Subscribe(e.EventType, () => command.RaiseCanExecuteChanged(), ThreadOption.UIThread, true));
                metadata.IfIs((AutoInvalidateOnSelection s) => EventAggregator.Subscribe(s.SelectionType, () => command.RaiseCanExecuteChanged(), ThreadOption.UIThread, true));
            }
        }
    }
}

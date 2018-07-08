using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Exceptions;
using Quantum.Services;
using Quantum.Utils;
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
            if (command.CommandMetadata == null) return;

            foreach (var metadata in command.CommandMetadata) {
                if (metadata == null) continue;
                if(metadata.GetType() == typeof(AutoInvalidateOnEvent)) {
                    ProcessAutoInvalidateOnEvent(command, (AutoInvalidateOnEvent)metadata);
                }
                else if(metadata.GetType() == typeof(AutoInvalidateOnSelection)) {
                    ProcessAutoInvalidateOnSelection(command, (AutoInvalidateOnSelection)metadata);
                }
            }
        }

        #region SubscribeToEvent

        private void ProcessAutoInvalidateOnEvent(ICommandBase command, AutoInvalidateOnEvent metadata)
        {
            if (metadata.EventType == null) return;
            if (!metadata.EventType.IsSubclassOfRawGeneric(typeof(CompositePresentationEvent<>))) {
                throw new UnexpectedTypeException(typeof(CompositePresentationEvent<>), metadata.EventType, $"Error trying to process AutoInvalidateOnEvent metadata : {metadata.EventType.Name} is not an event type.");
            }

            GetType().GetMethod("SubscribeCommandToEvent").MakeGenericMethod(new Type[] { metadata.EventType, metadata.EventType.GetBaseTypeGenericArgument(typeof(CompositePresentationEvent<>)) }).
                Invoke(this, new object[] { command });

        }

        public void SubscribeCommandToEvent<TEvent, TPayload>(ICommandBase command)
            where TEvent : CompositePresentationEvent<TPayload>
        {
            EventAggregator.GetEvent<TEvent>().Subscribe(args => command.RaiseCanExecuteChanged(), ThreadOption.PublisherThread, true);
        }

        #endregion SubscribeToEvent

        #region SubscribeToSelection

        private void ProcessAutoInvalidateOnSelection(ICommandBase command, AutoInvalidateOnSelection metadata)
        {
            if (metadata.SelectionType == null) return;
            if(!metadata.SelectionType.IsSubclassOfRawGeneric(typeof(SelectionBase<>))) {
                throw new UnexpectedTypeException(typeof(SelectionBase<>), metadata.SelectionType, $"Error trying to process AutoInvalidateOnSelection metadata : {metadata.SelectionType.Name} is not a selection type");
            }

            GetType().GetMethod("SubscribeCommandToSelection").MakeGenericMethod(new Type[] { metadata.SelectionType, metadata.SelectionType.GetBaseTypeGenericArgument(typeof(SelectionBase<>)) }).
                Invoke(this, new object[] { command });
        }

        public void SubscribeCommandToSelection<TSelection, TPayload>(ICommandBase command)
            where TSelection : SelectionBase<TPayload>
        {
            EventAggregator.GetEvent<TSelection>().Subscribe(selection => command.RaiseCanExecuteChanged(), ThreadOption.PublisherThread, true);
        }

        #endregion SubscribeToSelection
    }
}

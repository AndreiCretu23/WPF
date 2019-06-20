using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Exceptions;
using Quantum.Services;
using Quantum.Utils;
using Quantum.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.Command
{
    internal class CommandInvalidationManagerService : ServiceBase, ICommandInvalidationManagerService
    {
        private IList<Subscription> MultiCommandsSubscriptions { get; } = new List<Subscription>();

        public CommandInvalidationManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        
        public void ProcessInvalidators(object command)
        {
            command.AssertParameterNotNull("Internal error : A null command was parsed for invalidation processing.");

            if (command is IGlobalCommand globalCommand) {
                ProcessGlobalCommandInvalidators(globalCommand);
            }

            else if(command is IMultiGlobalCommand multiGlobalCommand) {
                ProcessMultiGlobalCommandInvalidators(multiGlobalCommand);
            }

            else {
                throw new Exception("Internal Error : An unknown command type was parsed for invalidation processing.");
            }
        }

        private void ProcessGlobalCommandInvalidators(IGlobalCommand command)
        {
            foreach(var metadata in command.Metadata.OfType<IAutoInvalidateMetadata>()) {
                metadata.AttachMetadataDefinition(EventAggregator, () => command.RaiseCanExecuteChanged());
            }
        }

        private void ProcessMultiGlobalCommandInvalidators(IMultiGlobalCommand multiGlobalCommand)
        {
            multiGlobalCommand.OnCommandsComputed += (oldCommands, newCommands) =>
            {
                var associatedInvalidationSubscriptions = MultiCommandsSubscriptions.Where(o => oldCommands.Contains(o.Object)).ToList();
                foreach(var subscription in associatedInvalidationSubscriptions) {
                    subscription.Break();
                }

                foreach(var command in newCommands) {
                    foreach(var metadata in command.Metadata.OfType<IAutoInvalidateMetadata>()) {
                        var token = metadata.AttachMetadataDefinition(EventAggregator, () => command.RaiseCanExecuteChanged());
                        MultiCommandsSubscriptions.Add(new Subscription()
                        {
                            Event = EventAggregator.GetEvent(metadata.EventType), 
                            Object = command, 
                            Token = token
                        });
                    }
                }
            };

            foreach(var metadata in multiGlobalCommand.Metadata.OfType<IAutoInvalidateMetadata>()) {
                metadata.AttachMetadataDefinition(EventAggregator, () => multiGlobalCommand.ComputeCommands());
            }
        }
    }
}

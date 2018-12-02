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

            if (command is IManagedCommand managedCommand) {
                ProcessManagedCommandInvalidators(managedCommand);
            }

            else if(command is IMultiManagedCommand multiManagedCommand) {
                ProcessMultiManagedCommandInvalidators(multiManagedCommand);
            }

            else {
                throw new Exception("Internal Error : An unknown command type was parsed for invalidation processing.");
            }
        }

        private void ProcessManagedCommandInvalidators(IManagedCommand command)
        {
            foreach(var metadata in command.Metadata) {
                metadata.IfIs((AutoInvalidateOnEvent e) => EventAggregator.Subscribe(e.EventType, () => command.RaiseCanExecuteChanged(), ThreadOption.UIThread, true));
                metadata.IfIs((AutoInvalidateOnSelection s) => EventAggregator.Subscribe(s.SelectionType, () => command.RaiseCanExecuteChanged(), ThreadOption.UIThread, true));
            }
        }

        private void ProcessMultiManagedCommandInvalidators(IMultiManagedCommand multiManagedCommand)
        {
            multiManagedCommand.OnCommandsComputed += (oldCommands, newCommands) =>
            {
                var associatedInvalidationSubscriptions = MultiCommandsSubscriptions.Where(o => oldCommands.Contains(o.Object)).ToList();
                foreach(var subscription in associatedInvalidationSubscriptions) {
                    subscription.Event.Unsubscribe(subscription.Token);
                }

                foreach(var command in newCommands) {
                    foreach(var metadata in command.Metadata) {
                        metadata.IfIs((AutoInvalidateOnEvent e) => MultiCommandsSubscriptions.Add(new Subscription() { Event = EventAggregator.GetEvent(e.EventType), Object = command, Token = EventAggregator.Subscribe(e.EventType, () => command.RaiseCanExecuteChanged(), ThreadOption.UIThread, true) }));
                        metadata.IfIs((AutoInvalidateOnSelection s) => MultiCommandsSubscriptions.Add(new Subscription() { Event = EventAggregator.GetEvent(s.SelectionType), Object = command, Token = EventAggregator.Subscribe(s.SelectionType, () => command.RaiseCanExecuteChanged(), ThreadOption.UIThread, true) }));
                    }
                }
            };

            foreach(var metadata in multiManagedCommand.Metadata) {
                metadata.IfIs((AutoInvalidateOnEvent e) => EventAggregator.Subscribe(e.EventType, () => multiManagedCommand.ComputeCommands(), ThreadOption.UIThread, true));
                metadata.IfIs((AutoInvalidateOnSelection s) => EventAggregator.Subscribe(s.SelectionType, () => multiManagedCommand.ComputeCommands(), ThreadOption.UIThread, true));
            }
        }
    }
}

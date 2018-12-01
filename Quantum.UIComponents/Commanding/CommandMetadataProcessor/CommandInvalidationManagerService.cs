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
            multiManagedCommand.OnCommandsComputed += subCommands =>
            {
                var previousInvalidationSubscriptions = MultiCommandsSubscriptions.Where(o => o.Object == multiManagedCommand).ToList();
                foreach (var subscription in previousInvalidationSubscriptions) {
                    subscription.Event.Unsubscribe(subscription.Token);
                    MultiCommandsSubscriptions.Remove(subscription);
                }
                
                foreach(var subCommand in subCommands) {
                    foreach(var metadata in subCommand.Metadata) {
                        metadata.IfIs((AutoInvalidateOnEvent e) => MultiCommandsSubscriptions.Add(new Subscription() {  Event = EventAggregator.GetEvent(e.EventType), Object = multiManagedCommand, Token = EventAggregator.Subscribe(e.EventType, () => subCommand.RaiseCanExecuteChanged(), ThreadOption.UIThread, true) }));
                        metadata.IfIs((AutoInvalidateOnSelection s) => MultiCommandsSubscriptions.Add(new Subscription() { Event = EventAggregator.GetEvent(s.SelectionType), Object = multiManagedCommand, Token = EventAggregator.Subscribe(s.SelectionType, () => subCommand.RaiseCanExecuteChanged(), ThreadOption.UIThread, true) }));
                    }
                }
            };
        }
    }
}

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Metadata
{
    public interface IBringIntoViewOnEvent : IStaticPanelMetadata
    {
        SubscriptionToken AttachToDefinition(IEventAggregator eventAggregator, Action action);
    }

    /// <summary>
    /// This metadata type can only be associated with a static panel definition.
    /// The panel associated with the static panel definition owner of this metadata type 
    /// will be brought into view when the event of the specified type is published.
    /// Also there can be an optional condition (depending by the payload) to determine if the panel 
    /// should be brought into view or not.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TPayload"></typeparam>
    [Mandatory(false)]
    [SupportsMultiple(true)]
    public class BringIntoViewOnEvent<TEvent, TPayload> : IBringIntoViewOnEvent
        where TEvent : CompositePresentationEvent<TPayload>
    {
        public Predicate<TPayload> Condition { get; }

        public BringIntoViewOnEvent() { }
        public BringIntoViewOnEvent(Predicate<TPayload> condition) { Condition = condition; }

        public SubscriptionToken AttachToDefinition(IEventAggregator eventAggregator, Action action)
        {
            if(Condition == null) {
                return eventAggregator.GetEvent<TEvent>().Subscribe(payLoad => action(), ThreadOption.UIThread);
            }

            return eventAggregator.GetEvent<TEvent>().Subscribe(payload =>
            {
                if (Condition(payload)) {
                    action();
                }
            }, ThreadOption.UIThread);
        }
    }

    /// <summary>
    /// /// <summary>
    /// This metadata type can only be associated with a static panel definition.
    /// The panel associated with the static panel definition owner of this metadata type 
    /// will be brought into view when the selection of the specified type is changes.
    /// Also there can be an optional condition (depending by the payload) to determine if the panel 
    /// should be brought into view or not.
    /// </summary>
    /// </summary>
    /// <typeparam name="TSelection"></typeparam>
    /// <typeparam name="TPayload"></typeparam>
    [Mandatory(false)]
    [SupportsMultiple(true)]
    public class BringIntoViewOnSelection<TSelection, TPayload> : IBringIntoViewOnEvent
        where TSelection : SelectionBase<TPayload>
    {
        public Predicate<TSelection> Condition { get; }

        public BringIntoViewOnSelection() { }
        public BringIntoViewOnSelection(Predicate<TSelection> condition) { Condition = condition; }

        public SubscriptionToken AttachToDefinition(IEventAggregator eventAggregator, Action action)
        {
            if(Condition == null) {
                return eventAggregator.GetEvent<TSelection>().Subscribe(s => action(), ThreadOption.UIThread);
            }

            return eventAggregator.GetEvent<TSelection>().Subscribe(s =>
            {
                if(Condition((TSelection)s)) {
                    action();
                }
            }, ThreadOption.UIThread);
        }
    }
}

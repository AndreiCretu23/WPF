using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Services;
using System;

namespace Quantum.Metadata
{
    public interface IAutoInvalidateMetadata : ICommandMetadata, IMultiCommandMetadata, ISubCommandMetadata, IToolBarMetadata, IStaticPanelMetadata, IDynamicPanelMetadata
    {
        Type EventType { get; }
        SubscriptionToken AttachMetadataDefinition(IEventAggregator eventAggregator, Action action, ThreadOption threadOption = ThreadOption.UIThread, bool keepSubscriberReferenceAlive = true); 
    }

    /// <summary>
    /// This metadata type hints the owner that an invalidation is required when the event of the specified type 
    /// in fired in the event aggregator instance of the application's container. The invalidation effect varies 
    /// depending on the owner of this metadata type. <para/>
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// </summary>
    [Mandatory(false)]
    [SupportsMultiple(true)]
    public class AutoInvalidateOnEvent<TEvent, TPayload> : IAutoInvalidateMetadata
        where TEvent : CompositePresentationEvent<TPayload>
    {
        public Type EventType => typeof(TEvent);
        private Predicate<TPayload> Condition { get; }

        public AutoInvalidateOnEvent() { }
        public AutoInvalidateOnEvent(Predicate<TPayload> condition) { Condition = condition; }

        public SubscriptionToken AttachMetadataDefinition(IEventAggregator eventAggregator, Action action, ThreadOption threadOption = ThreadOption.UIThread, bool keepSubscriberReferenceAlive = true)
        {
            if(Condition == null) {
                return eventAggregator.GetEvent<TEvent>().Subscribe(payload => action());
            }

            return eventAggregator.GetEvent<TEvent>().Subscribe(payload =>
            {
                if(Condition(payload)) {
                    action();
                }
            }, threadOption, keepSubscriberReferenceAlive);
        }
    }

    /// <summary>
    /// This metadata type hints the owner that an invalidation is required when the selection of the specified type 
    /// resolved from the the event aggregator instance of the application's container changes. 
    /// The invalidation effect varies depending on the owner of this metadata type. <para/>
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// </summary>
    [Mandatory(false)]
    [SupportsMultiple(true)]
    public class AutoInvalidateOnSelection<TSelection, TPayload> : IAutoInvalidateMetadata
        where TSelection : SelectionBase<TPayload>
    {
        public Type EventType => typeof(TSelection);
        private Predicate<TSelection> Condition { get; }

        public AutoInvalidateOnSelection() { }
        public AutoInvalidateOnSelection(Predicate<TSelection> condition) { Condition = condition; }

        public SubscriptionToken AttachMetadataDefinition(IEventAggregator eventAggregator, Action action, ThreadOption threadOption = ThreadOption.UIThread, bool keepSubscriberReferenceAlive = true)
        {
            if (Condition == null) {
                return eventAggregator.GetEvent<TSelection>().Subscribe(s => action(), ThreadOption.UIThread, true);
            }

            return eventAggregator.GetEvent<TSelection>().Subscribe(s =>
            {
               if(Condition((TSelection)s)) {
                    action();
               };
            }, threadOption, keepSubscriberReferenceAlive);
        }
    }

}

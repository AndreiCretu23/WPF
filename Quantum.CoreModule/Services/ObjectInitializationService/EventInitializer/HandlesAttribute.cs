using Microsoft.Practices.Composite.Presentation.Events;
using System;

namespace Quantum.Core.Services
{
    /// <summary>
    /// In objects initialized by the IObjectInitializationService, methods that are decorated with this attribute
    /// are automatically subscribed to the CompositePresentationEvent of the type specified in the constructor
    /// of the EventAggregator instance of the container. <para></para>
    /// 
    /// For example decorating a method with [Handles(typeof(SomeEvent)] is the equivalent of adding 
    /// <code>
    /// Container.Resolve IEventAggregator().GetEvent SomeEvent ().Subscribe(MethodName)
    /// </code>
    /// in the constructor. <para></para>
    /// 
    /// Of course, in order for the event/method to trigger there must also be a publisher that raises it : Container.Resolve IEventAggregator ().GetEvent SomeEvent ().Publish(new SomeEventArgs(...))
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class HandlesAttribute : Attribute
    {
        public Type EventType { get; private set; }
        public ThreadOption ThreadOption { get; private set; }
        public bool KeepSubscriberReferenceAlive { get; private set; }

        public bool IsThreadOptionSet { get; private set; }
        public bool IsKeepSubscriberReferenceAliveSet { get; set; }

        public HandlesAttribute(Type eventType)
        {
            this.EventType = eventType;
        }

        public HandlesAttribute(Type eventType, ThreadOption threadOption)
        {
            this.EventType = eventType;

            this.ThreadOption = threadOption;
            this.IsThreadOptionSet = true;
        }

        public HandlesAttribute(Type eventType, bool keepSubscriberReferenceAlive)
        {
            this.EventType = eventType;

            this.KeepSubscriberReferenceAlive = keepSubscriberReferenceAlive;
            this.IsKeepSubscriberReferenceAliveSet = true;
        }

        public HandlesAttribute(Type eventType, ThreadOption threadOption, bool keepSubscriberReferenceAlive)
        {
            this.EventType = eventType;

            this.ThreadOption = ThreadOption;
            this.IsThreadOptionSet = true;

            this.KeepSubscriberReferenceAlive = keepSubscriberReferenceAlive;
            this.IsKeepSubscriberReferenceAliveSet = true;
        }
    }
}

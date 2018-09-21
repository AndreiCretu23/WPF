using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Services;
using Quantum.Utils;

namespace Quantum.UIComponents
{
    /// <summary>
    /// In objects initialized by the IObjectInitializationService that extend ObservableObject, 
    /// properties which are decorated with this attribute will automatically be invalidated (RaisePropertyChanged will be called) 
    /// when the specified event is triggered from the EventAggregator instance of the container.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class InvalidateOnAttribute : Attribute
    {
        public Type EventType { get; }

        public InvalidateOnAttribute(Type eventType)
        {
            AssertType(eventType);
            EventType = eventType;
        }

        private void AssertType(Type eventType)
        {
            if(eventType == null)
            {
                throw new Exception($"Error : {typeof(InvalidateOnAttribute).Name} : EventType cannot be null!");
            }

            if(!(eventType.IsSubclassOfRawGeneric(typeof(CompositePresentationEvent<>)) || 
                 eventType.IsSubclassOfRawGeneric(typeof(SelectionBase<>))))
            {
                throw new Exception($"Error : {eventType.Name} is not a supported event type. \n" +
                                    $"Supported event types are types which are assignable from {typeof(CompositePresentationEvent<>).Name} or {typeof(SelectionBase<>).Name}.");
            }
        }
    }
}

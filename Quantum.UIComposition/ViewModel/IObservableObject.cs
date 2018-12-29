using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Quantum.UIComposition
{
    /// <summary>
    /// Defines the basic contact for a class which has UI notification support.
    /// </summary>
    public interface IObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Begins a block notification scope where the notifications are not sent to the listeners.
        /// The PropertyChanged requests are stored and when the scope ends, all of them will be sent to the listeners
        /// exactly once, even if more than one Request was made for a specific property.
        /// <code>
        /// using(obj.BeginBlockNotifications())
        /// {
        ///     obj.Prop1 = 1;
        ///     obj.Prop2 = 3;
        ///     obj.Prop2 = 6;
        /// } // Two events will be sent to the listeners : one for Prop1, one for Prop2.
        /// </code>
        /// </summary>
        /// <returns></returns>
        IDisposable BeginBlockNotifications();

        /// <summary>
        /// Raises the property changed event args for the property passed in as an expression. 
        /// <code>
        ///     RaisePropertyChanged(() => Property1); 
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propExpression"></param>
        void RaisePropertyChanged<T>(Expression<Func<T>> propExpression);

        /// <summary>
        /// Raises the property changed event for the property whose name is passed in as a parameter.
        /// </summary>
        /// <param name="propName"></param>
        void RaisePropertyChanged(string propName);

        /// <summary>
        /// Raises the property changed event for the specified PropertyChangedEventArgs.
        /// </summary>
        /// <param name="propArgs"></param>
        void RaisePropertyChanged(PropertyChangedEventArgs propArgs);

        // <summary>
        /// Raises the property changed event for each property owned by the type of this instance.
        /// </summary>
        void InvalidateAllProperties();
    }
}

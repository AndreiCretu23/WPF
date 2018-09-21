using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Quantum.UIComposition
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ScopedValue<bool> BlockNotificationsScope { get; set; }
        private Dictionary<string, PropertyChangedEventArgs> BlockedNotifications { get; set; }

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
        public IDisposable BeginBlockNotifications()
        {
            if(BlockNotificationsScope == null)
            {
                BlockNotificationsScope = new ScopedValue<bool>();
                BlockNotificationsScope.OnScopeEnd += (sender, e) => OnBlockNotificationsEnd();
            }
            BlockedNotifications = new Dictionary<string, PropertyChangedEventArgs>();
            return BlockNotificationsScope.BeginValueScope(true);
        }

        private void OnBlockNotificationsEnd()
        {
            foreach(var value in BlockedNotifications.Values)
            {
                RaisePropertyChanged(value);
            }
            BlockedNotifications.Clear();
        }

        /// <summary>
        /// Called when a property changes, exactly before the PropertyChanged event is fired. 
        /// Can be overriden in a derived class to provide custom logic for various cases.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) { }

        /// <summary>
        /// Raises the property changed event args for the property passed in as an expression. 
        /// <code>
        ///     RaisePropertyChanged(() => Property1); 
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propExpression"></param>
        public void RaisePropertyChanged<T>(Expression<Func<T>> propExpression)
        {
            propExpression.AssertParameterNotNull(nameof(propExpression));
            RaisePropertyChanged(ReflectionUtils.GetPropertyName(propExpression));
        }

        /// <summary>
        /// Raises the property changed event for the property whose name is passed in as a parameter.
        /// </summary>
        /// <param name="propName"></param>
        public void RaisePropertyChanged(string propName)
        {
            propName.AssertParameterNotNull(nameof(propName));
            RaisePropertyChanged(new PropertyChangedEventArgs(propName));
        }

        /// <summary>
        /// Raises the property changed event for the specified PropertyChangedEventArgs.
        /// </summary>
        /// <param name="propArgs"></param>
        public void RaisePropertyChanged(PropertyChangedEventArgs propArgs)
        {
            propArgs.AssertParameterNotNull(nameof(propArgs));

            if(BlockNotificationsScope == null || !BlockNotificationsScope.Value)
            {
                OnPropertyChanged(propArgs);
                PropertyChanged?.Invoke(this, propArgs);
            }

            else
            {
                if(!BlockedNotifications.ContainsKey(propArgs.PropertyName))
                {
                    BlockedNotifications.Add(propArgs.PropertyName, propArgs);
                }
            }
        }

        /// <summary>
        /// Raises the property changed event for each property owned by the type of this instance.
        /// </summary>
        public void InvalidateAllProperties()
        {
            foreach(var prop in GetType().GetProperties())
            {
                RaisePropertyChanged(prop.Name);
            }
        }
    }
}

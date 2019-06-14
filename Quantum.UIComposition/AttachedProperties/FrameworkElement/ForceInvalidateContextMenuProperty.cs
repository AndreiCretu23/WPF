using System;
using System.Windows;
using System.Windows.Data;
using UIFrameworkElement = System.Windows.FrameworkElement;
using UIItemsControl = System.Windows.Controls.ItemsControl;

namespace Quantum.AttachedProperties
{
    public static partial class FrameworkElement
    {

        public static readonly DependencyProperty ForceInvalidateContextMenuProperty = DependencyProperty.RegisterAttached
        (
            name: "ForceInvalidateContextMenu",
            propertyType: typeof(bool),
            ownerType: typeof(FrameworkElement),
            defaultMetadata: new PropertyMetadata(defaultValue: false, propertyChangedCallback: OnForceInvalidateContextMenuChanged)
        );


        public static bool GetForceInvalidateContextMenu(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(ForceInvalidateContextMenuProperty);
        }

        public static void SetForceInvalidateContextMenu(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(ForceInvalidateContextMenuProperty, value);
        }

        private static void OnForceInvalidateContextMenuChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if(!(obj is UIFrameworkElement))
            {
                throw new Exception("Error : ContextMenu.ForceInvalidate is only allowed on framework elements.");
            }

            var frameworkElement = (UIFrameworkElement)obj;
            var oldValue = (bool)e.OldValue;
            var newValue = (bool)e.NewValue;

            if(oldValue && !newValue)
            {
                frameworkElement.ContextMenuOpening -= InvalidationHandler;
            }

            else if(!oldValue && newValue)
            {
                frameworkElement.ContextMenuOpening += InvalidationHandler;
            }
        }

        private static void InvalidationHandler(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            var frameworkElement = (UIFrameworkElement)sender;
            if(frameworkElement.ContextMenu != null)
            {
                var bindingExpression = BindingOperations.GetBindingExpression(frameworkElement.ContextMenu, UIItemsControl.ItemsSourceProperty);
                if(bindingExpression != null)
                {
                    bindingExpression.UpdateTarget();
                }
            }
        }
        
    }
}

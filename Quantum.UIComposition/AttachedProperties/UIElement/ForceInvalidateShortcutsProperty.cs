using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using StandardUIElement = System.Windows.UIElement;

namespace Quantum.AttachedProperties
{
    public static partial class UIElement
    {

        public static readonly DependencyProperty ForceInvalidateShortcutsProperty = DependencyProperty.RegisterAttached
        (
            name: "ForceInvalidateShortcuts",
            propertyType: typeof(bool),
            ownerType: typeof(UIElement),
            defaultMetadata: new UIPropertyMetadata(false, OnForceInvalidateShortcutsChanged)
        );

        public static bool GetForceInvalidateShortcuts(DependencyObject obj)
        {
            return (bool)obj.GetValue(ForceInvalidateShortcutsProperty);
        }

        public static void SetForceInvalidateShortcuts(DependencyObject obj, bool value)
        {
            obj.SetValue(ForceInvalidateShortcutsProperty, value);
        }


        private static void OnForceInvalidateShortcutsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if(!(obj is StandardUIElement))
            {
                throw new Exception("Error : ForceInvalidateShortcutsProperty is only allowed on UIElements.");
            }

            var uiElement = (StandardUIElement)obj;
            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;

            if(oldValue && !newValue)
            {
                uiElement.KeyDown -= InvalidateShortcuts;
            }

            else if(!oldValue && newValue)
            {
                uiElement.KeyDown += InvalidateShortcuts;
            }
        }

        private static void InvalidateShortcuts(object sender, KeyEventArgs e)
        {
            var uiElement = (StandardUIElement)sender;

            var bindingExpression = BindingOperations.GetBindingExpression(uiElement, ShortcutsProperty);
            if(bindingExpression != null)
            {
                bindingExpression.UpdateTarget();
            }
        }
    }
}

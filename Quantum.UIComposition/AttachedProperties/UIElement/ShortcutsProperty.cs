using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using StandardUIElement = System.Windows.UIElement;

namespace Quantum.AttachedProperties
{
    public static partial class UIElement
    {
        public static readonly DependencyProperty ShortcutsProperty = DependencyProperty.RegisterAttached
        (
            name: "Shortcuts", 
            propertyType: typeof(IEnumerable<KeyBinding>),
            ownerType: typeof(UIElement), 
            defaultMetadata: new UIPropertyMetadata(Enumerable.Empty<KeyBinding>(), ShortcutsChanged)
        );
        
        public static IEnumerable<KeyBinding> GetShortcuts(DependencyObject obj)
        {
            return (IEnumerable<KeyBinding>)obj.GetValue(ShortcutsProperty);
        }

        public static void SetShortcuts(DependencyObject obj, IEnumerable<KeyBinding> value)
        {
            obj.SetValue(ShortcutsProperty, value);
        }
        
        private static void ShortcutsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if(!(obj is StandardUIElement uiElement)) {
                throw new Exception($"Error : ShortcutsProperty (Attached) can only be used on UIElements.");
            }

            uiElement.InputBindings.Clear();

            var keyBindings = (IEnumerable<KeyBinding>)e.NewValue;
            if(keyBindings == null) {
                return;
            }

            foreach(var keyBinding in keyBindings)
            {
                uiElement.InputBindings.Add(keyBinding);
            }
        }

    }
}

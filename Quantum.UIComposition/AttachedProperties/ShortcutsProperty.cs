using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Quantum.UIComposition
{
    public static partial class AttachedProperties
    {
        public static readonly DependencyProperty ShortcutsProperty =
            DependencyProperty.RegisterAttached("Shortcuts", typeof(IEnumerable<KeyBinding>),
                typeof(AttachedProperties), new UIPropertyMetadata(Enumerable.Empty<KeyBinding>(), ShortcutsChanged));

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
            var uiElement = obj as UIElement;
            if(uiElement == null) {
                throw new Exception($"Object of type '{obj.GetType().Name}' does not support InputBindings");
            }

            uiElement.InputBindings.Clear();

            var keyBindings = (IEnumerable<KeyBinding>)e.NewValue;
            foreach(var keyBinding in keyBindings)
            {
                uiElement.InputBindings.Add(keyBinding);
            }
        }

    }
}

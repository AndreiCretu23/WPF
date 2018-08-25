using Quantum.Utils;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Quantum.UIComposition
{
    public static partial class AttachedProperties
    {
        public static bool GetInheritInputBindingFromMainWindow(DependencyObject obj)
        {
            return (bool)obj.CheckedGetValue(InheritInputBindingFromMainWindowProperty);
        }

        public static void SetInheritInputBindingFromMainWindow(DependencyObject obj, bool value)
        {
            obj.CheckedSetValue(InheritInputBindingFromMainWindowProperty, value);
        }

        // Using a DependencyProperty as the backing store for InheritInputBindingFromMainWindo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InheritInputBindingFromMainWindowProperty =
            DependencyProperty.RegisterAttached("InheritInputBindingFromMainWindow", typeof(bool), typeof(AttachedProperties),
            new UIPropertyMetadata(OnInheritInputBindingFromMainWindowChanged));

        static void OnInheritInputBindingFromMainWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(true))
            {
                var uiElement = d as UIElement;
                var mainWindow = Application.Current.MainWindow;
                foreach (var item in mainWindow.InputBindings.Cast<InputBinding>())
                {
                    uiElement.InputBindings.Add(item);
                }
            }
        }
    }
}

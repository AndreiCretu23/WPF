using System;
using System.Windows;
using StandardUIElement = System.Windows.UIElement;
using UITextBox = System.Windows.Controls.TextBox;

namespace Quantum.AttachedProperties
{
    public static partial class TextBox
    {
        public static readonly DependencyProperty SelectAllOnFocusProperty = DependencyProperty.RegisterAttached
        (
            name: "SelectAllOnFocus",
            propertyType: typeof(bool),
            ownerType: typeof(TextBox),
            defaultMetadata: new PropertyMetadata(false, new PropertyChangedCallback(OnSelectAllOnFocusChanged))
        );

        public static bool GetSelectAllOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFocusProperty);
        }

        public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFocusProperty, value);
        }

        private static void OnSelectAllOnFocusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is UITextBox textBox)) {
                throw new Exception("SelectAllOnFocus attached property is only allowed on TextBox controls.");
            }

            if ((bool)e.OldValue) {
                textBox.RemoveHandler(StandardUIElement.GotFocusEvent, (RoutedEventHandler)SelectAllOnFocusHandler);
            }

            if ((bool)e.NewValue) {
                textBox.AddHandler(StandardUIElement.GotFocusEvent, (RoutedEventHandler)SelectAllOnFocusHandler);
            }
        }

        private static void SelectAllOnFocusHandler(object sender, RoutedEventArgs e)
        {
            var textBox = (UITextBox)sender;
            textBox.SelectAll();
        }
    }
}

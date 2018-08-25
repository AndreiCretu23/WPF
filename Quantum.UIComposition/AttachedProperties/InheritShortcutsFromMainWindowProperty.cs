﻿using Quantum.Utils;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Quantum.UIComposition
{
    [SuppressMessage("Microsoft.Design", "IDE0019")]
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
        
        public static readonly DependencyProperty InheritInputBindingFromMainWindowProperty =
            DependencyProperty.RegisterAttached("InheritInputBindingFromMainWindow", typeof(bool), typeof(AttachedProperties),
            new UIPropertyMetadata(OnInheritInputBindingFromMainWindowChanged));

        static void OnInheritInputBindingFromMainWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = d as FrameworkElement;
            if (frameworkElement == null)
            {
                throw new Exception("In order to inherit the shortcuts from the MainWindow, a binding must be created between " +
                                    "the InputBindingProperty of the UIElement and the InputBindingContext of the MainWindow." +
                                    "Only FrameWorkElements support binding.");
            }

            var mainWindow = Application.Current.MainWindow;
            var shortcutsBinding = BindingOperations.GetBinding(mainWindow, ShortcutsProperty);

            if(shortcutsBinding != null)
            {
                if (e.NewValue.Equals(true))
                {
                    frameworkElement.SetBinding(ShortcutsProperty, new Binding()
                    {
                        Path = shortcutsBinding.Path,
                        Source = mainWindow.DataContext,
                    });
                }
                else
                {
                    var currentBinding = BindingOperations.GetBinding(frameworkElement, ShortcutsProperty);
                    if(currentBinding != null && 
                       currentBinding.Source == shortcutsBinding.Source &&
                       currentBinding.Path == shortcutsBinding.Path)
                    {
                        BindingOperations.ClearBinding(frameworkElement, ShortcutsProperty);
                    }
                }
            }
        }
    }
}

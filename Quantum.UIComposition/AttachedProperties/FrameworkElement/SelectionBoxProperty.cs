﻿using Quantum.Controls;
using System;
using System.Windows;
using UIFrameworkElement = System.Windows.FrameworkElement;

namespace Quantum.AttachedProperties
{
    public static partial class FrameworkElement
    {
        #region SelectionBoxHandler

        private static readonly DependencyPropertyKey SelectionBoxHandlerPropertyKey = DependencyProperty.RegisterAttachedReadOnly
        (
            name: "SelectionBoxHandler",
            propertyType: typeof(SelectionBoxHandler),
            ownerType: typeof(FrameworkElement),
            defaultMetadata: new PropertyMetadata(defaultValue: null, propertyChangedCallback: OnSelectionBoxHandlerChanged)
        );

        public static readonly DependencyProperty SelectionBoxHandlerProperty = SelectionBoxHandlerPropertyKey.DependencyProperty;

        public static SelectionBoxHandler GetSelectionBoxHandler(DependencyObject obj)
        {
            return (SelectionBoxHandler)obj.GetValue(SelectionBoxHandlerProperty);
        }

        private static void SetSelectionBoxHandler(DependencyObject obj, SelectionBoxHandler value)
        {
            obj.SetValue(SelectionBoxHandlerPropertyKey, value);
        }

        public static void OnSelectionBoxHandlerChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(!(sender is UIFrameworkElement frameworkElement)) {
                throw new Exception("Error : Selection box handlers are only allowed on framework elements.");
            }
            
            if(e.OldValue is SelectionBoxHandler oldHandler) {
                oldHandler.Detach();
            }

            if(e.NewValue is SelectionBoxHandler newHandler) {
                newHandler.Attach();
            }
        }

        #endregion SelectionBoxHandler


        #region IsSelecting

        public static readonly DependencyProperty IsSelectingProperty = DependencyProperty.RegisterAttached
        (
            name: "IsSelecting",
            propertyType: typeof(bool),
            ownerType: typeof(FrameworkElement),
            defaultMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static bool GetIsSelecting(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsSelectingProperty);
        }

        public static void SetIsSelecting(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsSelectingProperty, value);
        }

        #endregion IsSelecting


        #region SelectionBox

        public static readonly DependencyProperty SelectionBoxProperty = DependencyProperty.RegisterAttached
        (
            name: "SelectionBox", 
            propertyType: typeof(SelectionBox),
            ownerType: typeof(FrameworkElement),
            defaultMetadata: new UIPropertyMetadata(null, SelectionBoxChanged)
        );

        public static SelectionBox GetSelectionBox(DependencyObject obj)
        {
            return (SelectionBox)obj.GetValue(SelectionBoxProperty);
        }

        public static void SetSelectionBox(DependencyObject obj, SelectionBox value)
        {
            obj.SetValue(SelectionBoxProperty, value);
        }
        
        public static void SelectionBoxChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            if(!(obj is UIFrameworkElement frameworkElement)) {
                throw new Exception("Error : Selection boxes are only allowed on framework elements.");
            }

            if(e.OldValue is SelectionBox oldSelectionBox) {
                oldSelectionBox.RemoveHandler(SelectionBox.SelectionStateChangedEvent, (SelectionStateChangedHandler)OnIsSelectingChanged);
            }

            if(e.NewValue is SelectionBox selectionBox) {
                SetSelectionBoxHandler(obj, new SelectionBoxHandler(frameworkElement, selectionBox));
                selectionBox.AddHandler(SelectionBox.SelectionStateChangedEvent, (SelectionStateChangedHandler)OnIsSelectingChanged);
            }

            else {
                SetSelectionBoxHandler(obj, null);
            }
        }

        public static void OnIsSelectingChanged(object sender, SelectionStateChangedArgs e)
        {
            SetIsSelecting(e.Owner, e.IsSelecting);
        }

        #endregion SelectionBoxHandler
    }
}

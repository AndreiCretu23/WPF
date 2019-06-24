using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UIItemsControl = System.Windows.Controls.ItemsControl;

namespace Quantum.Controls
{
    [DebuggerDisplay("Header = {Content}")]
    [ContentPart(Name = CustomContentHostTemplatePartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ContentHostTemplatePartName, Type = typeof(FrameworkElement))]
    public class TreeViewItem : UIItemsControl, ICustomContentOwner
    {
        #region TemplatePartNames

        public const string ContentHostTemplatePartName = "PART_ContentHost";
        public const string CustomContentHostTemplatePartName = "PART_CustomContentHost";

        #endregion TemplatePartNames


        #region DependencyProperties

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register
        (
            name: nameof(IsExpanded),
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty ToggleExpandOnDoubleClickProperty = DependencyProperty.Register
        (
            name: nameof(ToggleExpandOnDoubleClick),
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register
        (
            name: nameof(IsSelected),
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );


        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register
        (
            name: nameof(Content),
            propertyType: typeof(object),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: null)
        );
        
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register
        (
            name: nameof(ContentTemplate),
            propertyType: typeof(DataTemplate),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: null)
        );

        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register
        (
            name: nameof(ContentTemplateSelector),
            propertyType: typeof(DataTemplateSelector),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: null)
        );

        public static readonly DependencyPropertyKey ContentElementPropertyKey = DependencyProperty.RegisterReadOnly
        (
            name: nameof(ContentElement),
            propertyType: typeof(FrameworkElement),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: null)
        );

        #endregion DependencyProperties


        #region Properties

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public bool ToggleExpandOnDoubleClick
        {
            get { return (bool)GetValue(ToggleExpandOnDoubleClickProperty); }
            set { SetValue(ToggleExpandOnDoubleClickProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }
        
        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }
        
        public FrameworkElement ContentElement
        {
            get { return (FrameworkElement)GetValue(ContentElementPropertyKey.DependencyProperty); }
            private set { SetValue(ContentElementPropertyKey, value); }
        }

        #endregion Properties

        
        internal TreeView Root { get; }
        internal new UIItemsControl Parent { get; }

        internal TreeViewSelectionManager SelectionManager { get { return Root.SelectionManager; } }

        public TreeViewItem(TreeView root, UIItemsControl parent)
        {
            Root = root;
            Parent = parent;
        }
        
        static TreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(typeof(TreeViewItem)));
        }
        

        #region AssignPropertyChanged

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.Property == IsSelectedProperty) {
                SelectionManager.NotifySelectionChanged(this);
            }

            base.OnPropertyChanged(e);
        }

        #endregion AssignPropertyChanged


        #region Initialize

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!(Template.FindName(ContentHostTemplatePartName, this) is ContentControl contentControl))
            {
                throw new Exception("Error : The template set on this TreeViewItem does not have the mandatory \"PART_ContentHost\" template part.");
            }

            contentControl.EnsureLoaded(() =>
            {
                if (!(VisualTreeHelper.GetChild(contentControl, 0) is ContentPresenter contentPresenter))
                {
                    throw new Exception("Error : The content control template part of a TreeViewItem is expected to own a ContentPresenter.");
                }

                ContentElement = (ContentTemplate?.FindName(CustomContentHostTemplatePartName, contentPresenter)) as FrameworkElement ?? contentControl;
            });
        }

        #endregion Initialize


        #region ItemContainerConfig

        protected override bool ShouldApplyItemContainerStyle(DependencyObject container, object item)
        {
            return container is TreeViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItem(Root, this);
        }

        #endregion ItemContainerConfig


        #region Selection

        private void HandleMouseSelection(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) {
                if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift)) {
                    SelectionManager.SelectItemsBetweenLastSelectedAnd(this);
                    if(SelectionManager.IsMultipleSelection) { Root.Focus(); }
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift) {
                    SelectionManager.SelectOnlyItemsBetweenLastSelectedAnd(this);
                    if (SelectionManager.IsMultipleSelection) { Root.Focus(); }
                }
                else if (Keyboard.Modifiers == ModifierKeys.Control) {
                    SelectionManager.ToggleItemSelection(this);
                    if (SelectionManager.IsMultipleSelection) { Root.Focus(); }
                }
                else {
                    SelectionManager.SelectSingleItem(this);
                }
                e.Handled = true;
            }

            else if (e.ChangedButton == MouseButton.Right) {
                if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) &&
                    !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) &&
                    !SelectionManager.IsMultipleSelection) {
                    SelectionManager.SelectSingleItem(this);
                    e.Handled = true;
                }
            }
        }
        
        private void HandleDoubleClickToggleExpand(MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2 && ToggleExpandOnDoubleClick) {
                IsExpanded = !IsExpanded;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if(!IsWithinContent(e.OriginalSource as DependencyObject)) {
                return;
            }

            HandleMouseSelection(e);
            HandleDoubleClickToggleExpand(e);
            if(!IsFocused && !ContentElement.IsKeyboardFocusWithin) {
                Focus();
            }
        }

        private bool IsWithinContent(DependencyObject dependencyObject)
        {
            while(dependencyObject != null && dependencyObject != this) {
                if(dependencyObject == ContentElement) {
                    return true;
                }

                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            return false;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            SelectionManager.Clean();
        }

        #endregion Selection

    }
}

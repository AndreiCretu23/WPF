using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UIItemsControl = System.Windows.Controls.ItemsControl;

namespace Quantum.Controls
{
    [DebuggerDisplay("Header = {Header}")]
    [TemplatePart(Name = "PART_ContentHost", Type = typeof(FrameworkElement))]
    public class TreeViewItem : UIItemsControl, ICustomContentOwner
    {
        #region DependencyProperties

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register
        (
            name: "IsExpanded",
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty ToggleExpandOnDoubleClickProperty = DependencyProperty.Register
        (
            name: "ToggleExpandOnDoubleClick",
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register
        (
            name: "IsSelected",
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty IsCheckableProperty = DependencyProperty.Register
        (
            name: "IsCheckable",
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register
        (
            name: "IsChecked",
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register
        (
            name: "Icon",
            propertyType: typeof(ImageSource), 
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: null)
        );

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register
        (
            name: "Header",
            propertyType: typeof(string),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: null)
        );

        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register
        (
            name: "IsEditing",
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty EditableHeaderProperty = DependencyProperty.Register
        (
            name: "EditableHeader",
            propertyType: typeof(string),
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

        public bool IsCheckable
        {
            get { return (bool)GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }


        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public string EditableHeader
        {
            get { return (string)GetValue(EditableHeaderProperty); }
            set { SetValue(EditableHeaderProperty, value); }
        }

        #endregion Properties

        private FrameworkElement ContentElement { get; set; }

        internal TreeView Root { get; private set; }
        internal new UIItemsControl Parent { get; private set; }

        internal TreeViewSelectionManager SelectionManager { get { return Root.SelectionManager; } }

        public TreeViewItem()
        {
            Loaded += OnLoaded;
        }

        static TreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(typeof(TreeViewItem)));
        }


        #region Initialize

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var ancestors = this.GetVisualAncestors(o => o is TreeView || o is TreeViewItem).Cast<UIItemsControl>();
            if (!ancestors.OfType<TreeView>().IsSingleElement()) {
                throw new Exception("Error : TreeViewItem is only allowed as a visual child of a TreeView.");
            }

            Root = ancestors.OfType<TreeView>().First();
            Parent = ancestors.First();

            GetVisualContent().AddHandler(PreviewMouseDownEvent, (MouseButtonEventHandler)OnContentPreviewMouseDown);
            GetVisualContent().AddHandler(MouseDownEvent, (MouseButtonEventHandler)OnContentMouseDown);
            GetVisualContent().IsKeyboardFocusWithinChanged += OnContentIsKeyboardFocusWithinChanged;

            Loaded -= OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            GetVisualContent().RemoveHandler(PreviewMouseDownEvent, (MouseButtonEventHandler)OnContentPreviewMouseDown);
            GetVisualContent().RemoveHandler(MouseDownEvent, (MouseButtonEventHandler)OnContentMouseDown);
            GetVisualContent().IsKeyboardFocusWithinChanged -= OnContentIsKeyboardFocusWithinChanged;

            Unloaded -= OnUnloaded;
            Loaded += OnLoaded;
        }

        #endregion Initialize


        #region AssignPropertyChanged

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.Property == IsSelectedProperty) {
                OnSelectionChanged();
            }

            base.OnPropertyChanged(e);
        }

        #endregion AssignPropertyChanged


        #region ItemContainerConfig

        protected override bool ShouldApplyItemContainerStyle(DependencyObject container, object item)
        {
            return container is TreeViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItem();
        }

        public FrameworkElement GetVisualContent()
        {
            return ContentElement ?? this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ContentElement = Template.FindName("PART_ContentHost", this) as FrameworkElement;
        }

        #endregion ItemContainerConfig


        #region Selection

        private void OnSelectionChanged()
        {
            HandleExitEditMode();
            SelectionManager.NotifySelectionChanged(this);
        }

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
            }

            else if (e.ChangedButton == MouseButton.Right) {
                if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) &&
                    !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) &&
                    !SelectionManager.IsMultipleSelection) {
                    SelectionManager.SelectSingleItem(this);
                }
            }
        }

        #endregion Selection


        #region Keyboard

        private void OnContentPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HandleMouseSelection(e);
            HandleDoubleClickToggleExpand(e);
            if(!IsFocused && !GetVisualContent().IsKeyboardFocusWithin) {
                Focus();
            }
        }

        private void OnContentMouseDown(object sender, MouseButtonEventArgs e)
        {
            HandleExitEditMode();
        }

        private void OnContentIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false) {
                HandleExitEditMode();
            }

            base.OnIsKeyboardFocusWithinChanged(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if(e.Key == Key.Enter) {
                HandleExitEditMode();
            }
            
            if(MatchesSource(e) && IsKeyEventShortcut(e) && (!IsSelected || !IsFocused)) {
                e.Handled = true;
            }

            if(!e.Handled) {
                base.OnPreviewKeyDown(e);
            }
        }
        
        private bool MatchesSource(RoutedEventArgs e)
        {
            var originalSource = e.OriginalSource;
            if(!(originalSource is DependencyObject dependencyObject)) {
                return false;
            }

            else if(dependencyObject is TreeViewItem) {
                return originalSource == this;
            }
            
            else {
                return dependencyObject.GetVisualAncestorsOfType<TreeViewItem>().FirstOrDefault() == this;
            }
        }

        #endregion Keyboard


        #region Behavior

        private void HandleDoubleClickToggleExpand(MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2 && ToggleExpandOnDoubleClick) {
                IsExpanded = !IsExpanded;
            }
        }

        #endregion Behavior


        #region Editing

        private void HandleExitEditMode()
        {
            if(IsEditing) {
                IsEditing = false;
            }
        }

        #endregion Editing


        #region ContextMenu

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            if (SelectionManager.IsMultipleSelection || (MatchesSource(e) && (!IsSelected || !IsFocused))) {
                e.Handled = true;
                if(Root.ContextMenu != null && Root.ContextMenu.HasItems) {
                    Root.ContextMenu.IsOpen = true;
                }
            }
            else {
                base.OnContextMenuOpening(e);
            }
        }

        #endregion ContextMenu


        #region Shortcuts

        private bool IsKeyEventShortcut(KeyEventArgs e)
        {
            return InputBindings.OfType<KeyBinding>().Any(o => e.KeyboardDevice.Modifiers == o.Modifiers &&
                                                               e.KeyboardDevice.IsKeyDown(o.Key));
        }

        #endregion Shortcuts

    }
}

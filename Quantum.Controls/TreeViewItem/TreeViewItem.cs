using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
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

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register
        (
            name: "IsSelected",
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );
        
        #endregion DependencyProperties

        
        #region Properties

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
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

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
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

            Loaded -= OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
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

        #endregion ItemContainerConfig


        #region Selection
        
        private void OnSelectionChanged()
        {
            SelectionManager.NotifySelectionChanged(this);
        }

        #endregion Selection


        #region Keyboard

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) {
                if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift)) {
                    SelectionManager.SelectItemsBetweenLastSelectedAnd(this);
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift) {
                    SelectionManager.SelectOnlyItemsBetweenLastSelectedAnd(this);
                }
                else if (Keyboard.Modifiers == ModifierKeys.Control) {
                    SelectionManager.ToggleItemSelection(this);
                }
                else {
                    SelectionManager.SelectSingleItem(this);
                }
            }

            else if (e.ChangedButton == MouseButton.Right) {                
                if(!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && 
                   !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && 
                   !SelectionManager.IsMultipleSelection) {
                    SelectionManager.SelectSingleItem(this);
                }

            }
            
            e.Handled = true;
        }

        #endregion Keyboard


        #region Misc

        public FrameworkElement GetVisualContent()
        {
            return ContentElement ?? this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ContentElement = Template.FindName("PART_ContentHost", this) as FrameworkElement;
        }

        #endregion Misc

    }
}

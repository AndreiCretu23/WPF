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
    public class TreeViewItem : UIItemsControl
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
        
        public TreeView Root { get; private set; }
        public new UIItemsControl Parent { get; private set; }

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
            Root.NotifySelectionChanged(this);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) {
                if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift)) {
                    Root.SelectItemsBetweenLastSelectedAnd(this);
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift) {
                    Root.SelectOnlyItemsBetweenLastSelectedAnd(this);
                }
                else if (Keyboard.Modifiers == ModifierKeys.Control) {
                    Root.ToggleItemSelection(this);
                }
                else {
                    Root.SelectSingleItem(this);
                }
            }

            else if (e.ChangedButton == MouseButton.Right) {                
                if(!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && 
                   !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && 
                   !Root.IsMultipleSelection) {
                    Root.SelectSingleItem(this);
                }

            }
            
            e.Handled = true;
        }

        #endregion Selection


        #region Utils
        
        public IEnumerable<UIItemsControl> GetThisAndAncestors()
        {
            UIItemsControl parent = this;
            while(parent != null) {
                yield return parent;
                parent = parent is TreeViewItem tvi ? tvi.Parent : null;
            }
        }
        
        public IEnumerable<TreeViewItem> GetChildren()
        {
            for(int i = 0; i < Items.Count; i++) {
                if(ItemContainerGenerator.ContainerFromIndex(i) is TreeViewItem item) {
                    yield return item;
                }
            }
        }

        public TreeViewItem GetPrevious()
        {
            var currentIndex = Parent.ItemContainerGenerator.IndexFromContainer(this);

            if(currentIndex == 0) {
                if(Parent is TreeViewItem item) {
                    return item;
                }
                else {
                    return null;
                }
            }

            else {
                var prevContainer = (TreeViewItem)Parent.ItemContainerGenerator.ContainerFromIndex(currentIndex - 1);
                var prevContainerChildren = prevContainer.GetChildren();
                if(prevContainerChildren.Any()) {
                    return prevContainerChildren.Last();
                }
                return prevContainer;
            }
        }

        public TreeViewItem GetNext()
        {
            if (ItemContainerGenerator.ContainerFromIndex(0) is TreeViewItem firstChild) {
                return firstChild;
            }
            
            else if(Parent.ItemContainerGenerator.ContainerFromIndex(Parent.ItemContainerGenerator.IndexFromContainer(this) + 1) is TreeViewItem nextElement) {
                return nextElement;
            }
            
            else if(Parent is TreeViewItem parentTreeViewItem && parentTreeViewItem.Parent != null) {
                return parentTreeViewItem.Parent.ItemContainerGenerator.ContainerFromIndex(parentTreeViewItem.Parent.ItemContainerGenerator.IndexFromContainer(parentTreeViewItem) + 1) as TreeViewItem;
            }

            return null;
        }

        #endregion Utils

    }
}

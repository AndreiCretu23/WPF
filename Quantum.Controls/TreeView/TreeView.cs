using Quantum.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UIItemsControl = System.Windows.Controls.ItemsControl;

namespace Quantum.Controls
{
    public class TreeView : UIItemsControl
    {
        #region DependencyProperties

        public static readonly DependencyProperty AllowMultipleSelectionProperty = DependencyProperty.Register
        (
            name: "AllowMultipleSelection",
            propertyType: typeof(bool),
            ownerType: typeof(TreeView),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty AllowTabNavigationProperty = DependencyProperty.Register
        (
            name: "AllowTabNavigation",
            propertyType: typeof(bool),
            ownerType: typeof(TreeView),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty AllowArrowNavigationProperty = DependencyProperty.Register
        (
            name: "AllowArrowNavigation",
            propertyType: typeof(bool),
            ownerType: typeof(TreeView),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );
        
        #endregion DependencyProperties

        #region Properties

        public bool AllowMultipleSelection
        {
            get { return (bool)GetValue(AllowMultipleSelectionProperty); }
            set { SetValue(AllowMultipleSelectionProperty, value); }
        }

        public bool AllowTabNavigation
        {
            get { return (bool)GetValue(AllowTabNavigationProperty); }
            set { SetValue(AllowTabNavigationProperty, value); }
        }

        public bool AllowArrowNavigation
        {
            get { return (bool)GetValue(AllowArrowNavigationProperty); }
            set { SetValue(AllowArrowNavigationProperty, value); }
        }

        #endregion Properties

        public TreeView()
        {
        }

        static TreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeView), new FrameworkPropertyMetadata(typeof(TreeView)));
        }

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


        #region AssignPropertyChanged

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        #endregion AssignPropertyChanged


        #region Selection

        internal bool HasSelection { get { return SelectedItemsInternal.Any(); } }
        internal bool IsMultipleSelection { get { return SelectedItemsInternal.Count() > 1; } }

        private readonly ISet<TreeViewItem> SelectedItemsInternal = new HashSet<TreeViewItem>();
        
        internal void NotifySelectionChanged(TreeViewItem item)
        {
            if (item.IsSelected) {
                if(!AllowMultipleSelection) {
                    foreach (var treeViewItem in SelectedItemsInternal.ToHashSet()) {
                        treeViewItem.IsSelected = false;
                    }
                }

                SelectedItemsInternal.Add(item);
                item.Focus();
            }
            else {
                SelectedItemsInternal.Remove(item);
            }
        }

        internal void ClearSelection()
        {
            var items = SelectedItemsInternal.ToHashSet();
            foreach(var item in items) {
                UnselectItem(item);
            }
        }
        
        internal void ClearAllSelectedExcept(TreeViewItem item)
        {
            var items = SelectedItemsInternal.ToHashSet();
            if(items.Contains(item)) {
                items.Remove(item);
            }
            foreach(var treeViewItem in items) {
                UnselectItem(treeViewItem);
            }
        }

        internal void SelectItem(TreeViewItem item)
        {
            item.IsSelected = true;
        }

        internal void UnselectItem(TreeViewItem item)
        {
            item.IsSelected = false;
        }

        internal void ToggleItemSelection(TreeViewItem item)
        {
            item.IsSelected = !item.IsSelected;
        }

        internal void SelectSingleItem(TreeViewItem item)
        {
            if(item.IsSelected) {
                ClearAllSelectedExcept(item);
            }
            else {
                ClearSelection();
            }

            SelectItem(item);
        }

        internal void SelectItemsBetweenLastSelectedAnd(TreeViewItem item)
        {
            if (!AllowMultipleSelection) {
                SelectSingleItem(item);
            }

            if (!SelectedItemsInternal.Any()) {
                SelectItem(item);
            }
            
            else {
                var lastSelectedItem = SelectedItemsInternal.Last();
                
                var items = TreeViewTraverser.GetItemsBetween(lastSelectedItem, item);
                
                UnselectItem(lastSelectedItem);
                items.Remove(lastSelectedItem);
                items.Add(lastSelectedItem);

                foreach(var treeViewItem in items) {
                    SelectItem(treeViewItem);
                }
            }
        }

        internal void SelectOnlyItemsBetweenLastSelectedAnd(TreeViewItem item)
        {
            if(!AllowMultipleSelection) {
                SelectSingleItem(item);
            }

            if(!SelectedItemsInternal.Any()) {
                SelectItem(item);
            }
            else {
                var lastSelectedItem = SelectedItemsInternal.Last();
                ClearSelection();

                var items = TreeViewTraverser.GetItemsBetween(lastSelectedItem, item);

                UnselectItem(lastSelectedItem);
                items.Remove(lastSelectedItem);
                items.Add(lastSelectedItem);

                foreach(var treeViewItem in items) {
                    SelectItem(treeViewItem);
                }

            }
        }

        internal void SelectAllItems()
        {
            if (!AllowMultipleSelection) return;

            foreach(var item in this.GetVisualDescendantsOfType<TreeViewItem>()) {
                SelectItem(item);
            }
        }


        #endregion Selection


        #region Navigation

        private void HandleTabNavigation()
        {
            if (!AllowTabNavigation || !SelectedItemsInternal.Any()) return;

            var lastSelectedItem = SelectedItemsInternal.Last();
            var next = lastSelectedItem.GetNext();

            if (next != null) {
                SelectSingleItem(next);
            }
        }

        private void HandleBackTabNavigation()
        {
            if (!AllowTabNavigation || !SelectedItemsInternal.Any()) return;

            var lastSelectedItem = SelectedItemsInternal.Last();
            var previous = lastSelectedItem.GetPrevious();

            if (previous != null) {
                SelectSingleItem(previous);
            }
        }

        private void HandleArrowNavigation()
        {
            if (!AllowArrowNavigation || !SelectedItemsInternal.Any()) return;

            var lastSelectedItem = SelectedItemsInternal.Last();
            var next = lastSelectedItem.GetNext();

            if (next != null) {
                SelectSingleItem(next);
            }
        }

        private void HandleBackArrowNavigation()
        {
            if (!AllowArrowNavigation || !SelectedItemsInternal.Any()) return;

            var lastSelectedItem = SelectedItemsInternal.Last();
            var previous = lastSelectedItem.GetPrevious();

            if (previous != null) {
                SelectSingleItem(previous);
            }
        }



        // Shift + Arrow Navigation

        private TreeViewArrowNavigationHandler MultipleNavigationHandler = null;
        
        private void TeardownMultipleArrowNavigationSession()
        {
            MultipleNavigationHandler = null;    
        }

        private void HandleMultipleArrowNavigation()
        {
            if (!AllowArrowNavigation || !SelectedItemsInternal.Any()) return;
            
            if(!AllowMultipleSelection) {
                HandleArrowNavigation();
                return;
            }

            if(MultipleNavigationHandler == null) {
                MultipleNavigationHandler = new TreeViewArrowNavigationHandler(this, SelectedItemsInternal.Last(), TreeViewNavigationDirection.Down);
            }

            MultipleNavigationHandler.NavigateDown();
        }

        private void HandleMultipleBackArrowNavigation()
        {
            if (!AllowArrowNavigation || !SelectedItemsInternal.Any()) return;

            if(!AllowMultipleSelection) {
                HandleBackArrowNavigation();
                return;
            }

            if (MultipleNavigationHandler == null) {
                MultipleNavigationHandler = new TreeViewArrowNavigationHandler(this, SelectedItemsInternal.Last(), TreeViewNavigationDirection.Up);
            }

            MultipleNavigationHandler.NavigateUp();
        }
        
        #endregion Navigation


        #region Keyboard

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.KeyboardDevice.IsKeyDown(Key.A)) {
                SelectAllItems();
            }
            
            else if(e.KeyboardDevice.Modifiers == ModifierKeys.Shift && e.KeyboardDevice.IsKeyDown(Key.Tab)) {
                HandleBackTabNavigation();
            }

            else if(e.KeyboardDevice.Modifiers == ModifierKeys.Shift && e.KeyboardDevice.IsKeyDown(Key.Up)) {
                HandleMultipleBackArrowNavigation();
            }

            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift && e.KeyboardDevice.IsKeyDown(Key.Down)) {
                HandleMultipleArrowNavigation();
            }

            else if (e.KeyboardDevice.IsKeyDown(Key.Tab)) {
                HandleTabNavigation();
            }

            else if (e.KeyboardDevice.IsKeyDown(Key.Up)) {
                HandleBackArrowNavigation();
            }

            else if (e.KeyboardDevice.IsKeyDown(Key.Down)) {
                HandleArrowNavigation();
            }

            else {
                base.OnKeyDown(e);
                return;
            }

            e.Handled = true;
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if((e.Key == Key.LeftShift || e.Key == Key.RightShift) && MultipleNavigationHandler != null) {
                TeardownMultipleArrowNavigationSession();
            }
            base.OnPreviewKeyUp(e);
        }

        #endregion Keyboard

    }
}

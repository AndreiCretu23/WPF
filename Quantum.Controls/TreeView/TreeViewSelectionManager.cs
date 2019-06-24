using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quantum.Controls
{
    internal class TreeViewSelectionManager
    {
        public TreeView TreeView { get; }
        
        private bool AllowMultipleSelection { get { return TreeView.AllowMultipleSelection; } }

        internal bool HasSelection { get { return SelectedItemsInternal.Any(); } }
        internal bool IsSingleSelection { get { return HasSelection && !IsMultipleSelection; } }
        internal bool IsMultipleSelection { get { return SelectedItemsInternal.Count() > 1; } }

        private readonly ISet<TreeViewItem> SelectedItemsInternal = new HashSet<TreeViewItem>();
        internal IEnumerable<TreeViewItem> SelectedItems { get { return SelectedItemsInternal; } }

        public TreeViewSelectionManager(TreeView treeView)
        {
            TreeView = treeView;
        }
        
        internal void NotifySelectionChanged(TreeViewItem item)
        {
            if (item.IsSelected) {
                if (!AllowMultipleSelection) {
                    foreach (var treeViewItem in SelectedItemsInternal.ToHashSet()) {
                        treeViewItem.IsSelected = false;
                    }
                }

                SelectedItemsInternal.Add(item);
                item.Focus();
            }
            else {
                SelectedItemsInternal.Remove(item);
                if(HasSelection && !IsMultipleSelection) {
                    SelectedItemsInternal.Single().Focus();
                }
            }
        }

        internal void ClearSelection()
        {
            var items = SelectedItemsInternal.ToHashSet();
            foreach (var item in items) {
                UnselectItem(item);
            }
        }

        internal void ClearAllSelectedExcept(TreeViewItem item)
        {
            var items = SelectedItemsInternal.ToHashSet();
            if (items.Contains(item)) {
                items.Remove(item);
            }
            foreach (var treeViewItem in items) {
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
            if (item.IsSelected) {
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

                foreach (var treeViewItem in items) {
                    SelectItem(treeViewItem);
                }
            }
        }

        internal void SelectOnlyItemsBetweenLastSelectedAnd(TreeViewItem item)
        {
            if (!AllowMultipleSelection) {
                SelectSingleItem(item);
            }

            if (!SelectedItemsInternal.Any()) {
                SelectItem(item);
            }
            else {
                var lastSelectedItem = SelectedItemsInternal.Last();
                ClearSelection();

                var items = TreeViewTraverser.GetItemsBetween(lastSelectedItem, item);

                UnselectItem(lastSelectedItem);
                items.Remove(lastSelectedItem);
                items.Add(lastSelectedItem);

                foreach (var treeViewItem in items) {
                    SelectItem(treeViewItem);
                }

            }
        }

        internal void SelectAllItems()
        {
            if (!AllowMultipleSelection) return;

            foreach (var item in TreeView.GetVisualDescendantsOfType<TreeViewItem>()) {
                SelectItem(item);
            }
        }

        internal void Clean()
        {
            foreach(var item in SelectedItemsInternal.ToList()) {
                if(!(item.IsVisualChildOf(TreeView))) {
                    SelectedItemsInternal.Remove(item);
                }
            }
        }
    }
}

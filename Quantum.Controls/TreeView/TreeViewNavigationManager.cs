using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Controls
{
    public class TreeViewNavigationManager
    {
        internal TreeView TreeView { get; }
        private TreeViewSelectionManager SelectionManager { get { return TreeView.SelectionManager; } }
        private bool AllowMultipleSelection { get { return TreeView.AllowMultipleSelection; } }
        private bool AllowTabNavigation { get { return TreeView.AllowTabNavigation; } }
        private bool AllowArrowNavigation { get { return TreeView.AllowArrowNavigation; } }

        private TreeViewArrowNavigationHandler MultipleNavigationHandler = null;

        internal TreeViewNavigationManager(TreeView treeView)
        {
            TreeView = treeView;
        }

        internal void HandleTabNavigation()
        {
            if (!AllowTabNavigation || !SelectionManager.SelectedItems.Any()) return;

            var lastSelectedItem = SelectionManager.SelectedItems.Last();
            var next = lastSelectedItem.GetNext();

            if (next != null) {
                SelectionManager.SelectSingleItem(next);
            }
        }

        internal void HandleBackTabNavigation()
        {
            if (!AllowTabNavigation || !SelectionManager.SelectedItems.Any()) return;

            var lastSelectedItem = SelectionManager.SelectedItems.Last();
            var previous = lastSelectedItem.GetPrevious();

            if (previous != null) {
                SelectionManager.SelectSingleItem(previous);
            }
        }

        internal void HandleArrowNavigation()
        {
            if (!AllowArrowNavigation || !SelectionManager.SelectedItems.Any()) return;

            var lastSelectedItem = SelectionManager.SelectedItems.Last();
            var next = lastSelectedItem.GetNext();

            if (next != null) {
                SelectionManager.SelectSingleItem(next);
            }
        }

        internal void HandleBackArrowNavigation()
        {
            if (!AllowArrowNavigation || !SelectionManager.SelectedItems.Any()) return;

            var lastSelectedItem = SelectionManager.SelectedItems.Last();
            var previous = lastSelectedItem.GetPrevious();

            if (previous != null) {
                SelectionManager.SelectSingleItem(previous);
            }
        }



        // Shift + Arrow Navigation

        internal void HandleMultipleArrowNavigation()
        {
            if (!AllowArrowNavigation || !SelectionManager.SelectedItems.Any()) return;

            if (!AllowMultipleSelection) {
                HandleArrowNavigation();
                return;
            }

            if (MultipleNavigationHandler == null) {
                MultipleNavigationHandler = new TreeViewArrowNavigationHandler(TreeView, SelectionManager.SelectedItems.Last(), TreeViewNavigationDirection.Down);
            }

            MultipleNavigationHandler.NavigateDown();
        }

        internal void HandleMultipleBackArrowNavigation()
        {
            if (!AllowArrowNavigation || !SelectionManager.SelectedItems.Any()) return;

            if (!AllowMultipleSelection) {
                HandleBackArrowNavigation();
                return;
            }

            if (MultipleNavigationHandler == null) {
                MultipleNavigationHandler = new TreeViewArrowNavigationHandler(TreeView, SelectionManager.SelectedItems.Last(), TreeViewNavigationDirection.Up);
            }

            MultipleNavigationHandler.NavigateUp();
        }

        internal void TeardownMultipleArrowNavigationSession()
        {
            if(MultipleNavigationHandler != null) {
                MultipleNavigationHandler = null;
            }
        }
    }
}

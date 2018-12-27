namespace Quantum.Controls
{
    internal class TreeViewArrowNavigationHandler
    {
        internal TreeView Root { get; }
        private TreeViewSelectionManager SelectionManager { get { return Root.SelectionManager; } }

        private TreeViewItem Origin { get; }
        private TreeViewNavigationDirection Direction { get; set; }
        private TreeViewItem CurrentNavigationItem { get; set; }
        private bool ShouldNavigationSelect { get; set; }
        
        internal TreeViewArrowNavigationHandler(TreeView root, TreeViewItem origin, TreeViewNavigationDirection initialDirection)
        {
            Root = root;
            Origin = origin;
            Direction = initialDirection;
            CurrentNavigationItem = Origin;
            ShouldNavigationSelect = true;
        }
        
        internal void NavigateUp()
        {
            if (Direction == TreeViewNavigationDirection.Down) {
                Direction = TreeViewNavigationDirection.Up;
                ShouldNavigationSelect = !ShouldNavigationSelect;
            }

            TreeViewItem lastSelectedItem = CurrentNavigationItem;
            var previous = lastSelectedItem.GetPrevious();

            if (previous == null) return;

            if (previous != Origin) {
                if (ShouldNavigationSelect) {
                    SelectionManager.SelectItem(previous);
                }
                else {
                    SelectionManager.UnselectItem(lastSelectedItem);
                }
            }
            else {
                SelectionManager.UnselectItem(lastSelectedItem);
                ShouldNavigationSelect = !ShouldNavigationSelect;
            }

            CurrentNavigationItem = previous;
            CurrentNavigationItem.Focus();
        }


        internal void NavigateDown()
        {
            if (Direction == TreeViewNavigationDirection.Up) {
                Direction = TreeViewNavigationDirection.Down;
                ShouldNavigationSelect = !ShouldNavigationSelect;
            }

            TreeViewItem lastSelectedItem = CurrentNavigationItem;
            var next = lastSelectedItem.GetNext();

            if (next == null) return;

            if (next != Origin) {
                if (ShouldNavigationSelect) {
                    SelectionManager.SelectItem(next);
                }
                else {
                    SelectionManager.UnselectItem(lastSelectedItem);
                }
            }
            else {
                SelectionManager.UnselectItem(lastSelectedItem);
                ShouldNavigationSelect = !ShouldNavigationSelect;
            }

            CurrentNavigationItem = next;
            CurrentNavigationItem.Focus();
        }


    }
}

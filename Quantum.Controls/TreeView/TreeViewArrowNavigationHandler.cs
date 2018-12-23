namespace Quantum.Controls
{
    internal class TreeViewArrowNavigationHandler
    {
        private TreeView Root { get; }
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
                    Root.SelectItem(previous);
                }
                else {
                    Root.UnselectItem(lastSelectedItem);
                }
            }
            else {
                Root.UnselectItem(lastSelectedItem);
                ShouldNavigationSelect = !ShouldNavigationSelect;
            }

            CurrentNavigationItem = previous;
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
                    Root.SelectItem(next);
                }
                else {
                    Root.UnselectItem(lastSelectedItem);
                }
            }
            else {
                Root.UnselectItem(lastSelectedItem);
                ShouldNavigationSelect = !ShouldNavigationSelect;
            }

            CurrentNavigationItem = next;
        }


    }
}

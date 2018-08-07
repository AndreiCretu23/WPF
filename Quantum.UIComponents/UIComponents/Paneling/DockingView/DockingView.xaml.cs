using System.Windows.Controls;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    internal partial class DockingView : UserControl, IDockingView
    {
        public DockingManager DockingManager { get { return DockManager; } }
        public LayoutAnchorablePane UpperLeftArea { get { return UpperLeftPane; } }
        public LayoutAnchorablePane BottomLeftArea { get { return BottomLeftPane; } }
        public LayoutAnchorablePane CenterArea { get { return CenterPane; } }
        public LayoutAnchorablePane UpperRightArea { get { return UpperRightPane; } }
        public LayoutAnchorablePane BottomRightArea { get { return BottomRightPane; } }

        public DockingView()
        {
            InitializeComponent();
        }
    }
}

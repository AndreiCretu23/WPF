using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    public interface IDockingView
    {
        DockingManager DockingManager { get; }
        LayoutRoot LayoutRoot { get; }
        LayoutAnchorablePane UpperLeftArea { get; }
        LayoutAnchorablePane BottomLeftArea { get; }
        LayoutAnchorablePane CenterArea { get; }
        LayoutAnchorablePane UpperRightArea { get; }
        LayoutAnchorablePane BottomRightArea { get; }
    }
}

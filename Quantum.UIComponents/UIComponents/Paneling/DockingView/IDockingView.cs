using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    public interface IDockingView
    {
        LayoutAnchorablePane UpperLeftArea { get; }
        LayoutAnchorablePane BottomLeftArea { get; }
        LayoutAnchorablePane CenterArea { get; }
        LayoutAnchorablePane UpperRightArea { get; }
        LayoutAnchorablePane BottomRightArea { get; }
    }
}

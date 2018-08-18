using System;

namespace Quantum.Metadata
{
    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class StaticPanelConfiguration : IStaticPanelMetadata
    {
        public Func<string> Title { get; set; } = () => "StaticPanel";
        public Func<bool> CanFloat { get; set; } = () => true;
        public Func<bool> CanClose { get; set; } = () => true;
        public Func<bool> CanOpen { get; set; } = () => true;
        public Func<bool> IsVisible { get; set; } = () => true;
        public PanelPlacement Placement { get; set; } = PanelPlacement.Center;
    }

    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class DynamicPanelConfiguration<TViewModel> : IDynamicPanelMetadata
    {
        public Func<TViewModel, string> Title { get; set; } = o => "DynamicPanel";
        public Func<TViewModel, bool> CanFloat { get; set; } = o => true;
        public PanelPlacement Placement { get; set; } = PanelPlacement.Center;
    }

    public enum PanelPlacement
    {
        TopLeft, 
        BottomLeft, 
        Center, 
        TopRight, 
        BottomRight
    }

}

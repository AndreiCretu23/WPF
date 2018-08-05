using System;

namespace Quantum.Metadata
{
    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class StaticPanelConfiguration : IStaticPanelMetadata
    {
        public Func<bool> CanFloat { get; set; } = () => true;
        public Func<bool> CanClose { get; set; } = () => true;
        public Func<bool> CanOpen { get; set; } = () => true;
        public Func<bool> IsVisible { get; set; } = () => true;
        public PanelPlacement Placement { get; set; } = PanelPlacement.Center;
    }

    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class DynamicPanelConfiguration : IDynamicPanelMetadata
    {
        public Func<bool> CanFloat { get; set; } = () => true;
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

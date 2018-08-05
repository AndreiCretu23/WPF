using System;

namespace Quantum.Metadata
{
    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class StaticPanelConfiguration : IStaticPanelMetadata
    {
        public Func<bool> CanFloat { get; set; }
        public Func<bool> CanClose { get; set; }
        public Func<bool> IsVisible { get; set; }
        public PanelPlacement Placement { get; set; }
    }

    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class DynamicPanelConfiguration : IDynamicPanelMetadata
    {
        public Func<bool> CanFloat { get; set; }
        public PanelPlacement Placement { get; set; }
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

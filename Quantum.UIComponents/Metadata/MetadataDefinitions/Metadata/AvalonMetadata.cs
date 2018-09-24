using System;

namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type is used to configure the appearence and behavior of a static panel.
    /// </summary>
    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class StaticPanelConfiguration : IStaticPanelMetadata
    {
        /// <summary>
        /// A delegate that returns the title of the panel.
        /// </summary>
        public Func<string> Title { get; set; } = () => "StaticPanel";

        /// <summary>
        /// A delegate that returns a value indicating if the panel can float or not.
        /// </summary>
        public Func<bool> CanFloat { get; set; } = () => true;

        /// <summary>
        /// A delegate that returns a value indicating if the panel can be closed or not.
        /// </summary>
        public Func<bool> CanClose { get; set; } = () => true;

        /// <summary>
        /// A delegate that returns a value indicating if the panel can be opened or not.
        /// </summary>
        public Func<bool> CanOpen { get; set; } = () => true;

        /// <summary>
        /// A delegate that returns a value indicating if the panel is visible or not.
        /// </summary>
        public Func<bool> IsVisible { get; set; } = () => true;

        /// <summary>
        /// Returns a value indicating the initial position of the panel.
        /// </summary>
        public PanelPlacement Placement { get; set; } = PanelPlacement.Center;
    }

    /// <summary>
    /// This metadata type is used to configure the appearence and behavior of a dynamic panel collection.
    /// </summary>
    /// <typeparam name="T">The view or viewModel type of the panel.</typeparam>
    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class DynamicPanelConfiguration<T> : IDynamicPanelMetadata
    {
        /// <summary>
        /// A delegate that returns the title of a panel in the dynamic panel collection, 
        /// receiving the the view / viewModel instance(depending on the configuration type) as a parameter.
        /// </summary>
        public Func<T, string> Title { get; set; } = o => "DynamicPanel";

        /// <summary>
        /// A delegate that returns a value indicating if the panel can float or not, 
        /// receiving the view / viewModel instance(depending on the configuration type) as a parameter.
        /// </summary>
        public Func<T, bool> CanFloat { get; set; } = o => true;

        /// <summary>
        /// Returns a value indicating the prefered location of the dynamic panel collection.
        /// </summary>
        public PanelPlacement Placement { get; set; } = PanelPlacement.Center;
    }
    
    /// <summary>
    /// Contains region definitions for the placement of the panels inside the DockingManager.
    /// </summary>
    public enum PanelPlacement
    {
        TopLeft, 
        BottomLeft, 
        Center, 
        TopRight, 
        BottomRight
    }

}

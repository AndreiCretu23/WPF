using Quantum.Metadata;
using System;

namespace Quantum.UIComponents
{
    public interface IToolBarDefinition : ICloneable
    {
        /// <summary>
        /// The Band on which the associated View will be located on the ToolBar.
        /// </summary>
        int Band { get; set; }

        /// <summary>
        /// The index of the position where the view will be placed on the band.
        /// </summary>
        int BandIndex { get; set; }

        /// <summary>
        /// A delegate returning the logical value determining whether the associated ToolBar view is Visible or not.
        /// </summary>
        Func<bool> Visibility { get; }

        /// <summary>
        /// A collection of AutoEvent/Selection invalidators. Whenever an event in this collection triggers / a selection changes
        /// the ToolBar Visibility delegate will be re-evaluated.
        /// </summary>
        ToolBarMetadataCollection ToolBarMetadata { get; }

        /// <summary>
        /// The View type.
        /// </summary>
        Type View { get; }

        /// <summary>
        /// The interface type implemented by the view.
        /// </summary>
        Type IView { get; }

        /// <summary>
        /// The ViewModel type.
        /// </summary>
        Type ViewModel { get; }

        /// <summary>
        /// The interface type implemented by the ViewModel.
        /// </summary>
        Type IViewModel { get; }
    }
}

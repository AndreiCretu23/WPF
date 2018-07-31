using System;
using System.Windows.Controls;
using Quantum.Metadata;
using Quantum.Utils;

namespace Quantum.UIComponents
{
    public class ToolBarDefinition<ITView, TView, ITViewModel, TViewModel> : IToolBarDefinition
        where TView : UserControl, ITView, new()
        where ITView : class
        where TViewModel : class, ITViewModel
        where ITViewModel : class
    {
        /// <summary>
        /// The Band on which the associated View will be located on the ToolBar.
        /// </summary>
        public int Band { get; set; }

        /// <summary>
        /// The index of the position where the view will be placed on the band.
        /// </summary>
        public int BandIndex { get; set; }

        /// <summary>
        /// A delegate returning the logical value determining whether the associated ToolBar view is Visible or not.
        /// </summary>
        public Func<bool> Visibility { get; set; }

        /// <summary>
        /// A collection of AutoEvent/Selection invalidators. Whenever an event in this collection triggers / a selection changes
        /// the ToolBar Visibility delegate will be re-evaluated.
        /// </summary>
        public ToolBarMetadataCollection ToolBarMetadata { get; set; } = new ToolBarMetadataCollection();
        
        /// <summary>
        /// The View type.
        /// </summary>
        public Type View => typeof(TView);

        /// <summary>
        /// The interface type implemented by the view.
        /// </summary>
        public Type IView => typeof(ITView);
        
        /// <summary>
        /// The ViewModel type.
        /// </summary>
        public Type ViewModel => typeof(TViewModel);

        /// <summary>
        /// The interface type implemented by the ViewModel.
        /// </summary>
        public Type IViewModel => typeof(ITViewModel);
        
        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var metadata = new ToolBarMetadataCollection();
            ToolBarMetadata.ForEach(_ => metadata.Add(_));

            return new ToolBarDefinition<ITView, TView, ITViewModel, TViewModel>()
            {
                Band = Band,
                BandIndex = BandIndex, 
                ToolBarMetadata = metadata
            };
        }

        public ToolBarDefinition()
        {
        }
    }
}

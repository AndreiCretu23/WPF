using System;
using System.Windows.Controls;
using Quantum.Metadata;

namespace Quantum.UIComponents
{
    public class ToolBarDefinition<ITView, TView, ITViewModel, TViewModel> : IToolBarDefinition
        where TView : UserControl, ITView, new()
        where ITView : class
        where TViewModel : class, ITViewModel
        where ITViewModel : class
    {
        public int Band { get; set; }
        public int BandIndex { get; set; }
        public Func<bool> Visibility { get; set; }
        public ToolBarMetadataCollection ToolBarMetadata { get; set; } = new ToolBarMetadataCollection();

        public Type View => typeof(TView);
        public Type IView => typeof(ITView);
        public Type ViewModel => typeof(TViewModel);
        public Type IViewModel => typeof(ITViewModel);

        public ToolBarDefinition()
        {
        }
    }
}

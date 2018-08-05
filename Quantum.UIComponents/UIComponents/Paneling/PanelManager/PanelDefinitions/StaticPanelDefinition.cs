using Quantum.Metadata;
using System;
using System.Windows.Controls;

namespace Quantum.UIComponents
{
    [MandatoryCollection(false)]
    public class StaticPanelDefinition<ITView, TView, ITViewModel, TViewModel> : MetadataCollection<IStaticPanelMetadata>, IStaticPanelDefinition
        where TView : UserControl, ITView, new()
        where ITView : class
        where TViewModel : class, ITViewModel
        where ITViewModel : class
    {
        public Type IView => typeof(ITView);
        public Type View => typeof(TView);
        public Type IViewModel => typeof(ITViewModel);
        public Type ViewModel => typeof(TViewModel);
    }
}

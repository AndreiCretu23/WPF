using Quantum.Common;
using Quantum.Metadata;
using System;
using System.Windows.Controls;

namespace Quantum.UIComponents
{
    public class DynamicPanelDefinition<ITView, TView, ITViewModel, TViewModel> : MetadataCollection<IDynamicPanelMetadata>, IDynamicPanelDefinition
        where TView : UserControl, ITView, new()
        where ITView : class
        where TViewModel : class, ITViewModel, IIdentifiable
        where ITViewModel : class
    {
        public Type IView => typeof(ITView);
        public Type View => typeof(TView);
        public Type IViewModel => typeof(ITViewModel);
        public Type ViewModel => typeof(TViewModel);
    }
}

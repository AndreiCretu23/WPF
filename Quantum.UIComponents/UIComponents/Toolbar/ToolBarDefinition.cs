using System;
using System.Windows.Controls;
using Quantum.Metadata;

namespace Quantum.UIComponents
{
    public class ToolBarDefinition<ITView, TView, ITViewModel, TViewModel>
        where TView : UserControl, ITView
        where TViewModel : ITViewModel
    {
        public int Band { get; private set; }
        public int BandIndex { get; private set; }
        public Func<bool> Visibility { get; private set; }
        public ToolBarMetadataCollection ToolBarMetadata { get; private set; } = new ToolBarMetadataCollection();

        public ToolBarDefinition()
        {
        }
    }
}

using Quantum.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.UIComponents
{
    public interface IToolBarDefinition
    {
        int Band { get; set; }
        int BandIndex { get; set; }
        Func<bool> Visibility { get; }
        ToolBarMetadataCollection ToolBarMetadata { get; }

        Type View { get; }
        Type IView { get; }
        Type ViewModel { get; }
        Type IViewModel { get; }
    }
}

using Quantum.Metadata;
using System.Collections.Generic;

namespace Quantum.UIComponents
{
    public interface IDynamicPanelDefinition : IPanelDefinition, IEnumerable<IDynamicPanelMetadata>
    {
    }
}

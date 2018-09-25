using Quantum.Metadata;
using System.Collections.Generic;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Provides the basic contract for a DynamicPanelDefinition.
    /// </summary>
    public interface IDynamicPanelDefinition : IPanelDefinition, IEnumerable<IDynamicPanelMetadata>
    {
    }
}

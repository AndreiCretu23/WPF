using Quantum.Metadata;
using System.Collections.Generic;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Provides the basic contract for a StaticPanelDefinition.
    /// </summary>
    public interface IStaticPanelDefinition : IPanelDefinition, IEnumerable<IStaticPanelMetadata>
    {
    }
}

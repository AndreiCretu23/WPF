using Quantum.Metadata;
using System.Collections.Generic;

namespace Quantum.UIComponents
{
    public interface IStaticPanelDefinition : IPanelDefinition, IEnumerable<IStaticPanelMetadata>
    {
    }
}

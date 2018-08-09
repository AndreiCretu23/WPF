using Quantum.Metadata;
using Quantum.Utils;
using System.Linq;

namespace Quantum.UIComponents
{
    internal static class PanelDefinitionExt
    {
        public static bool CanChangeVisibility(this IStaticPanelDefinition definition, bool currentVisibility)
        {
            definition.AssertNotNull(nameof(definition));

            var panelConfig = definition.OfType<StaticPanelConfiguration>().Single();
            return currentVisibility ? panelConfig.CanClose() : panelConfig.CanOpen();
        }
    }
}

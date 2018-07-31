using Microsoft.Practices.Composite.Presentation.Events;
using System.Collections.Generic;

namespace Quantum.UIComponents
{
    internal class ToolBarLayoutRestoreRequest : CompositePresentationEvent<ToolBarLayoutRestoreArgs>
    {
    }

    internal class ToolBarLayoutRestoreArgs
    {
        public IEnumerable<IToolBarDefinition> DefaultDefinitions { get; private set; }
        
        public ToolBarLayoutRestoreArgs(IEnumerable<IToolBarDefinition> defaultDefinitions)
        {
            DefaultDefinitions = defaultDefinitions;
        }
    }
}

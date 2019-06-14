using Quantum.Command;
using Quantum.Metadata;
using System.Collections.Generic;

namespace Quantum.UIComponents
{
    internal interface IMainMenuCommandExtractor
    {
        IEnumerable<IGlobalCommand> GlobalCommands { get; }
        IEnumerable<IMultiGlobalCommand> MultiGlobalCommands { get; }
        IEnumerable<IStaticPanelDefinition> StaticPanelDefinitions { get; }

        IEnumerable<AbstractMenuPath> AbstractMenuPaths { get; }

        TMetadata GetMenuMetadata<TMetadata>(IGlobalCommand globalCommand) where TMetadata : IMainMenuMetadata;
        TMetadata GetMultiMenuMetadata<TMetadata>(IMultiGlobalCommand multiGlobalCommand) where TMetadata : IMultiMainMenuMetadata;
        TMetadata GetSubmenuMetadata<TMetadata>(ISubCommand subCommand) where TMetadata : ISubMainMenuMetadata;
        TMetadata GetPanelMenuOptionMetadata<TMetadata>(IStaticPanelDefinition definition) where TMetadata : IPanelMenuEntryMetadata;
    }
}

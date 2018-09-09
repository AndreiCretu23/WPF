using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quantum.Command;
using Quantum.Metadata;
using Quantum.Services;

namespace Quantum.UIComponents
{
    internal interface IMainMenuCommandExtractor
    {
        IEnumerable<IManagedCommand> ManagedCommands { get; }
        IEnumerable<IMultiManagedCommand> MultiManagedCommands { get; }
        IEnumerable<IStaticPanelDefinition> StaticPanelDefinitions { get; }

        IEnumerable<AbstractMenuPath> AbstractMenuPaths { get; }

        TMetadata GetMenuMetadata<TMetadata>(IManagedCommand managedCommand) where TMetadata : IMainMenuMetadata;
        TMetadata GetMultiMenuMetadata<TMetadata>(IMultiManagedCommand multiManagedCommand) where TMetadata : IMultiMainMenuMetadata;
        TMetadata GetSubmenuMetadata<TMetadata>(ISubCommand subCommand) where TMetadata : ISubMainMenuMetadata;
        TMetadata GetPanelMenuOptionMetadata<TMetadata>(IStaticPanelDefinition definition) where TMetadata : IPanelMenuEntryMetadata;
    }
}

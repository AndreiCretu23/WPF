using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quantum.Command;
using Quantum.Services;
using Quantum.Metadata;

namespace Quantum.UIComponents
{
    internal class MainMenuCommandExtractor : QuantumServiceBase, IMainMenuCommandExtractor
    {
        [Service]
        public ICommandManagerService CommandManager { get; set; }

        [Service]
        public IPanelManagerService PanelManager { get; set; }
        
        public IEnumerable<IManagedCommand> ManagedCommands => CommandManager.ManagedCommands.Where(c => c.Metadata.OfType<MainMenuOption>().Any());
        public IEnumerable<IMultiManagedCommand> MultiManagedCommands => CommandManager.MultiManagedCommands.Where(c => c.Metadata.OfType<MultiMainMenuOption>().Any());
        public IEnumerable<IStaticPanelDefinition> StaticPanelDefinitions => PanelManager.StaticPanelDefinitions.Where(def => def.OfType<PanelMenuOption>().Any());

        public MainMenuCommandExtractor(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        private IEnumerable<AbstractMenuPath> abstractMenuPaths;
        public IEnumerable<AbstractMenuPath> AbstractMenuPaths
        {
            get
            {
                if (abstractMenuPaths == null)
                {
                    abstractMenuPaths = ManagedCommands.Select(c => GetMenuMetadata<MenuPath>(c).ParentPath)
                                .Concat(MultiManagedCommands.Select(c => GetMultiMenuMetadata<MenuPath>(c).ParentPath))
                                .Concat(StaticPanelDefinitions.Select(def => GetPanelMenuOptionMetadata<MenuPath>(def).ParentPath)).
                             SelectMany(path => path.GetPathsToRoot()).Distinct();
                }
                return abstractMenuPaths;
            }
        }


        public TMetadata GetMenuMetadata<TMetadata>(IManagedCommand managedCommand) where TMetadata : IMainMenuMetadata
        {
            return managedCommand.Metadata.OfType<MainMenuOption>().Single().OfType<TMetadata>().SingleOrDefault();
        }

        public TMetadata GetMultiMenuMetadata<TMetadata>(IMultiManagedCommand multiManagedCommand) where TMetadata : IMultiMainMenuMetadata
        {
            return multiManagedCommand.Metadata.OfType<MultiMainMenuOption>().Single().OfType<TMetadata>().SingleOrDefault();
        }

        public TMetadata GetSubmenuMetadata<TMetadata>(ISubCommand subCommand) where TMetadata : ISubMainMenuMetadata
        {
            return subCommand.Metadata.OfType<SubMainMenuOption>().Single().OfType<TMetadata>().SingleOrDefault();
        }

        public TMetadata GetPanelMenuOptionMetadata<TMetadata>(IStaticPanelDefinition definition) where TMetadata : IPanelMenuEntryMetadata
        {
            return definition.OfType<PanelMenuOption>().Single().OfType<TMetadata>().SingleOrDefault();
        }

    }
}

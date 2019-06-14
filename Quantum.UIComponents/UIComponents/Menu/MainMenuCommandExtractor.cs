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
    internal class MainMenuCommandExtractor : ServiceBase, IMainMenuCommandExtractor
    {
        [Service]
        public ICommandManagerService CommandManager { get; set; }

        [Service]
        public IPanelManagerService PanelManager { get; set; }
        
        public IEnumerable<IGlobalCommand> GlobalCommands => CommandManager.GlobalCommands.Where(c => c.Metadata.OfType<MainMenuOption>().Any());
        public IEnumerable<IMultiGlobalCommand> MultiGlobalCommands => CommandManager.MultiGlobalCommands.Where(c => c.Metadata.OfType<MultiMainMenuOption>().Any());
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
                    abstractMenuPaths = GlobalCommands.Select(c => GetMenuMetadata<MenuPath>(c).ParentPath)
                                .Concat(MultiGlobalCommands.Select(c => GetMultiMenuMetadata<MenuPath>(c).ParentPath))
                                .Concat(StaticPanelDefinitions.Select(def => GetPanelMenuOptionMetadata<MenuPath>(def).ParentPath)).
                             SelectMany(path => path.GetPathsToRoot()).Distinct();
                }
                return abstractMenuPaths;
            }
        }


        public TMetadata GetMenuMetadata<TMetadata>(IGlobalCommand globalCommand) where TMetadata : IMainMenuMetadata
        {
            return globalCommand.Metadata.OfType<MainMenuOption>().Single().OfType<TMetadata>().SingleOrDefault();
        }

        public TMetadata GetMultiMenuMetadata<TMetadata>(IMultiGlobalCommand multiGlobalCommand) where TMetadata : IMultiMainMenuMetadata
        {
            return multiGlobalCommand.Metadata.OfType<MultiMainMenuOption>().Single().OfType<TMetadata>().SingleOrDefault();
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

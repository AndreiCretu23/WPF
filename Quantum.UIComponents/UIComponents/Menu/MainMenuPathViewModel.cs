using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Command;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.UIComponents
{
    internal class MainMenuPathViewModel : ViewModelBase, IMainMenuItemViewModel
    {
        public AbstractMenuPath MenuPath { get; }
        public IMainMenuCommandExtractor CommandExtractor { get; }
        
        #region Properties

        public string Header => MenuPath.Description?.Value;
        public string Icon => MenuPath.Icon?.IconPath;
        public string ToolTip => MenuPath.ToolTip?.Value;

        public IEnumerable<IMainMenuItemViewModel> Children => CreateChildren();

        #endregion Properties

        public MainMenuPathViewModel(IObjectInitializationService initSvc, IMainMenuCommandExtractor commandExtractor, AbstractMenuPath menuPath)
            : base(initSvc)
        {
            CommandExtractor = commandExtractor;
            MenuPath = menuPath;
            SubscribeToMultiCommandChildrenAutoInvalidationEvents();
        }


        public IEnumerable<IMainMenuItemViewModel> CreateChildren()
        {
            var children = new List<IMainMenuItemViewModel>();

            var managedCommands = CommandExtractor.ManagedCommands.Where(c => CommandExtractor.GetMenuMetadata<MenuPath>(c).ParentPath == MenuPath);
            var multiManagedCommands = CommandExtractor.MultiManagedCommands.Where(c => CommandExtractor.GetMultiMenuMetadata<MenuPath>(c).ParentPath == MenuPath);
            var subAbstractMenuPaths = CommandExtractor.AbstractMenuPaths.Where(path => path.ParentPath == MenuPath);
            var panelMenuOptions = CommandExtractor.StaticPanelDefinitions.Where(def => def.OfType<PanelMenuOption>().Any() && def.OfType<PanelMenuOption>().Single().OfType<MenuPath>().Single().ParentPath == MenuPath);

            var rawChildren = new Dictionary<IMenuEntry, object>();
            managedCommands.ForEach(c => rawChildren.Add(CommandExtractor.GetMenuMetadata<MenuPath>(c), c));
            multiManagedCommands.ForEach(c => rawChildren.Add(CommandExtractor.GetMultiMenuMetadata<MenuPath>(c), c));
            subAbstractMenuPaths.ForEach(path => rawChildren.Add(path, path));
            panelMenuOptions.ForEach(o => rawChildren.Add(CommandExtractor.GetPanelMenuOptionMetadata<MenuPath>(o), o));

            int categoryIndex = 0;
            int categoriesCount = rawChildren.Select(o => o.Key.CategoryIndex).Distinct().Count();
            var childrenByCategories = rawChildren.GroupBy(o => o.Key.CategoryIndex).OrderBy(o => o.Key);
            foreach (var category in childrenByCategories)
            {
                categoryIndex++;
                foreach (var entry in category.OrderBy(o => o.Key.OrderIndex))
                {
                    entry.Value.IfIs((IManagedCommand c) => children.Add(new MainMenuCommandViewModel(InitializationService, CommandExtractor, c)));

                    entry.Value.IfIs((IMultiManagedCommand c) =>
                    {
                        var subCommands = c.ComputeCommands().Where(subCmd => subCmd.Metadata.OfType<SubMainMenuOption>().Any());
                        foreach (var subCommand in subCommands)
                        {
                            children.Add(new MainMenuSubCommandViewModel(InitializationService, CommandExtractor, subCommand));
                        }
                    });

                    entry.Value.IfIs((IStaticPanelDefinition def) => children.Add(new MainMenuPanelEntryViewModel(InitializationService, CommandExtractor, def)));

                    entry.Value.IfIs((AbstractMenuPath p) =>  children.Add(new MainMenuPathViewModel(InitializationService, CommandExtractor, p))); 
                }

                if (categoryIndex < categoriesCount)
                {
                    children.Add(new MainMenuSeparatorViewModel());
                }
            }

            
            return children;
        }


        #region Misc
        
        private void SubscribeToMultiCommandChildrenAutoInvalidationEvents()
        {
            var childMultiCommandsMetadata = CommandExtractor.MultiManagedCommands.Where(c => CommandExtractor.GetMultiMenuMetadata<MenuPath>(c).ParentPath == MenuPath).
                                                                  SelectMany(c => c.Metadata);

            foreach (var metadata in childMultiCommandsMetadata)
            {
                metadata.IfIs((AutoInvalidateOnEvent e) => EventAggregator.Subscribe(e.EventType, () => RaisePropertyChanged(() => Children), ThreadOption.UIThread, true));
                metadata.IfIs((AutoInvalidateOnSelection s) => EventAggregator.Subscribe(s.SelectionType, () => RaisePropertyChanged(() => Children), ThreadOption.UIThread, true));
            }
        }

        #endregion Misc

    }
}

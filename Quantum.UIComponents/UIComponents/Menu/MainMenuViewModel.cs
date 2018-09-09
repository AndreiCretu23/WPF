﻿using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Command;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.UIComponents
{
    internal class MainMenuViewModel : ViewModelBase, IMainMenuViewModel
    {
        [Service]
        public ICommandManagerService CommandManager { get; set; }
   
        [Service]
        public IPanelManagerService PanelManager { get; set; }
        

        private IEnumerable<IManagedCommand> ManagedCommands => CommandManager.ManagedCommands.Where(c => c.Metadata.OfType<MainMenuOption>().Any());
        private IEnumerable<IMultiManagedCommand> MultiManagedCommands => CommandManager.MultiManagedCommands.Where(c => c.Metadata.OfType<MultiMainMenuOption>().Any());
        private IEnumerable<IStaticPanelDefinition> StaticPanelDefinitions => PanelManager.StaticPanelDefinitions.Where(def => def.OfType<PanelMenuOption>().Any());

        public MainMenuViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public IEnumerable<IMainMenuItemViewModel> Children { get { return CreateMenuContent(); } }

        private IEnumerable<IMainMenuItemViewModel> CreateMenuContent()
        {
            var rootPaths = AbstractMenuPaths.Where(path => path.ParentPath == AbstractMenuPath.Root).OrderBy(path => path.OrderIndex);
            foreach(var rootPath in rootPaths)
            {
                var viewModel = new MainMenuItemViewModel(rootPath)
                {
                    Header = rootPath.Description.Value,
                    ChildrenDelegate = GetChildrenDelegate()
                };

                SubscribeToMultiCommandChildrenAutoInvalidationEvents(viewModel);
                yield return viewModel;
            }
        }


        private Func<MainMenuItemViewModel, IEnumerable<IMainMenuItemViewModel>> GetChildrenDelegate()
        {
            Func<MainMenuItemViewModel, IEnumerable<IMainMenuItemViewModel>> getChildren = null;

            getChildren = viewModelItem =>
            {
                if(!(viewModelItem.MenuEntry is AbstractMenuPath)) {
                    return Enumerable.Empty<IMainMenuItemViewModel>();
                }

                var abstractMenuPath = viewModelItem.MenuEntry as AbstractMenuPath;
                var children = new List<IMainMenuItemViewModel>();

                var managedCommands = ManagedCommands.Where(c => GetMenuMetadata<MenuPath>(c).ParentPath == abstractMenuPath);
                var multiManagedCommands = MultiManagedCommands.Where(c => GetMultiMenuMetadata<MenuPath>(c).ParentPath == abstractMenuPath);
                var subAbstractMenuPaths = AbstractMenuPaths.Where(path => path.ParentPath == abstractMenuPath);
                var panelMenuOptions = StaticPanelDefinitions.Where(def => def.OfType<PanelMenuOption>().Any() && def.OfType<PanelMenuOption>().Single().OfType<MenuPath>().Single().ParentPath == abstractMenuPath);

                var rawChildren = new Dictionary<IMenuEntry, object>();
                managedCommands.ForEach(c => rawChildren.Add(GetMenuMetadata<MenuPath>(c), c));
                multiManagedCommands.ForEach(c => rawChildren.Add(GetMultiMenuMetadata<MenuPath>(c), c));
                subAbstractMenuPaths.ForEach(path => rawChildren.Add(path, path));
                panelMenuOptions.ForEach(o => rawChildren.Add(GetPanelMenuOptionMetadata<MenuPath>(o), o));

                int categoryIndex = 0;
                int categoriesCount = rawChildren.Select(o => o.Key.CategoryIndex).Distinct().Count();
                var childrenByCategories = rawChildren.GroupBy(o => o.Key.CategoryIndex).OrderBy(o => o.Key);
                foreach(var category in childrenByCategories)
                {
                    categoryIndex++;
                    foreach(var entry in category.OrderBy(o => o.Key.OrderIndex))
                    {
                        entry.Value.IfIs((IManagedCommand c) =>
                        {
                            children.Add(new MainMenuItemViewModel(entry.Key)
                            {
                                Command = c,
                                Header = GetMenuMetadata<Description>(c)?.Value,
                                Icon = GetMenuMetadata<Icon>(c)?.IconPath,
                                ToolTip = GetMenuMetadata<ToolTip>(c)?.Value,
                                Shortcut = c.Metadata.OfType<KeyShortcut>().SingleOrDefault()?.GetInputGestureText(),
                                IsCheckable = GetMenuMetadata<Checkable>(c)?.Value ?? false,
                                CheckedChanged = GetMenuMetadata<CheckChanged>(c)?.OnCheckChanged,
                                ChildrenDelegate = o => Enumerable.Empty<IMainMenuItemViewModel>()
                            });
                        });

                        entry.Value.IfIs((IMultiManagedCommand c) =>
                        {
                            var subCommands = c.ComputeCommands().Where(subCmd => subCmd.Metadata.OfType<SubMainMenuOption>().Any());
                            foreach(var subCommand in subCommands)
                            {
                                children.Add(new MainMenuItemViewModel(entry.Key)
                                {
                                    Command = subCommand,
                                    Header = GetSubmenuMetadata<Description>(subCommand)?.Value,
                                    Icon = GetSubmenuMetadata<Icon>(subCommand)?.IconPath,
                                    ToolTip = GetSubmenuMetadata<ToolTip>(subCommand)?.Value,
                                    IsCheckable = GetSubmenuMetadata<Checkable>(subCommand)?.Value ?? false,
                                    CheckedChanged = GetSubmenuMetadata<CheckChanged>(subCommand)?.OnCheckChanged,
                                    ChildrenDelegate = o => Enumerable.Empty<IMainMenuItemViewModel>()
                                });
                            }
                        });

                        entry.Value.IfIs((IStaticPanelDefinition def) =>
                        {
                            children.Add(new MainMenuPanelEntryViewModel(InitializationService, def));
                        });

                        entry.Value.IfIs((AbstractMenuPath p) =>
                        {
                            var childViewModel = new MainMenuItemViewModel(entry.Key)
                            {
                                Header = p.Description?.Value, 
                                Icon = p.Icon?.IconPath, 
                                ChildrenDelegate = getChildren
                            };
                            SubscribeToMultiCommandChildrenAutoInvalidationEvents(childViewModel);
                            children.Add(childViewModel);
                        });
                    }

                    if(categoryIndex < categoriesCount)
                    {
                        children.Add(new MainMenuSeparatorViewModel());
                    }
                }


                return children;
            };

            return getChildren;
        }




        #region Utils

        private IEnumerable<AbstractMenuPath> abstractMenuPaths;
        private IEnumerable<AbstractMenuPath> AbstractMenuPaths
        {
            get
            {
                if (abstractMenuPaths == null) {
                    abstractMenuPaths = ManagedCommands.Select(c => GetMenuMetadata<MenuPath>(c).ParentPath)
                                .Concat(MultiManagedCommands.Select(c => GetMultiMenuMetadata<MenuPath>(c).ParentPath))
                                .Concat(StaticPanelDefinitions.Select(def => GetPanelMenuOptionMetadata<MenuPath>(def).ParentPath)).
                             SelectMany(path => path.GetPathsToRoot()).Distinct();
                }
                return abstractMenuPaths;
            }
        }

        
        private TMetadata GetMenuMetadata<TMetadata>(IManagedCommand managedCommand) where TMetadata : IMainMenuMetadata
        {
            return managedCommand.Metadata.OfType<MainMenuOption>().Single().OfType<TMetadata>().SingleOrDefault();
        }

        private TMetadata GetMultiMenuMetadata<TMetadata>(IMultiManagedCommand multiManagedCommand) where TMetadata : IMultiMainMenuMetadata
        {
            return multiManagedCommand.Metadata.OfType<MultiMainMenuOption>().Single().OfType<TMetadata>().SingleOrDefault();
        }

        private TMetadata GetSubmenuMetadata<TMetadata>(ISubCommand subCommand) where TMetadata : ISubMainMenuMetadata
        {
            return subCommand.Metadata.OfType<SubMainMenuOption>().Single().OfType<TMetadata>().SingleOrDefault();
        }

        private TMetadata GetPanelMenuOptionMetadata<TMetadata>(IStaticPanelDefinition definition) where TMetadata : IPanelMenuEntryMetadata
        {
            return definition.OfType<PanelMenuOption>().Single().OfType<TMetadata>().SingleOrDefault();
        }

        private void SubscribeToMultiCommandChildrenAutoInvalidationEvents(MainMenuItemViewModel viewModelItem)
        {
            var abstractMenuPath = viewModelItem.MenuEntry as AbstractMenuPath;

            if (abstractMenuPath == null){
                return;
            }

            var childMultiCommandsMetadata = MultiManagedCommands.Where(c => GetMultiMenuMetadata<MenuPath>(c).ParentPath == abstractMenuPath).
                                                                  SelectMany(c => c.Metadata);

            foreach(var metadata in childMultiCommandsMetadata)
            {
                metadata.IfIs((AutoInvalidateOnEvent e) => EventAggregator.Subscribe(e.EventType, () => viewModelItem.RaiseChildrenChanged(), ThreadOption.UIThread, true));
                metadata.IfIs((AutoInvalidateOnSelection s) => EventAggregator.Subscribe(s.SelectionType, () => viewModelItem.RaiseChildrenChanged(), ThreadOption.UIThread, true));
            }
        }
        
        #endregion Utils


    }
}

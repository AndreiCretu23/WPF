using Microsoft.Practices.Composite.Presentation.Events;
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
        public IMetadataAsserterService MetadataAsserter { get; set; }

        [Service]
        public ICommandMetadataProcessorService MetadataProcessor { get; set; }
        
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

                var managedCommands = CommandManager.ManagedCommands.Where(c => GetMenuMetadata<MenuPath>(c).ParentPath == abstractMenuPath);
                var multiManagedCommands = CommandManager.MultiManagedCommands.Where(c => GetMultiMenuMetadata<MenuPath>(c).ParentPath == abstractMenuPath);
                var subAbstractMenuPaths = AbstractMenuPaths.Where(path => path.ParentPath == abstractMenuPath);

                var rawChildren = new Dictionary<IMenuEntry, object>();
                managedCommands.ForEach(c => rawChildren.Add(GetMenuMetadata<MenuPath>(c), c));
                multiManagedCommands.ForEach(c => rawChildren.Add(GetMultiMenuMetadata<MenuPath>(c), c));
                subAbstractMenuPaths.ForEach(path => rawChildren.Add(path, path));

                int categoryIndex = 0;
                int categoriesCount = rawChildren.Select(o => o.Key.CategoryIndex).Distinct().Count();
                var childrenByCategories = rawChildren.GroupBy(o => o.Key.CategoryIndex).OrderBy(o => o.Key);
                foreach(var category in childrenByCategories)
                {
                    categoryIndex++;
                    foreach(var entry in category.OrderBy(o => o.Key.OrderIndex))
                    {
                        entry.Value.IfIs((ManagedCommand c) =>
                        {
                            children.Add(new MainMenuItemViewModel(entry.Key)
                            {
                                Command = c,
                                Header = GetMenuMetadata<Description>(c)?.Value,
                                Icon = GetMenuMetadata<Icon>(c)?.IconPath,
                                ToolTip = GetMenuMetadata<ToolTip>(c)?.Value,
                                Shortcut = GetMenuMetadata<KeyShortcut>(c)?.GetInputGestureText(),
                                IsCheckable = GetMenuMetadata<Checkable>(c)?.Value ?? false,
                                CheckedChanged = GetMenuMetadata<CheckChanged>(c)?.OnCheckChanged,
                                ChildrenDelegate = o => Enumerable.Empty<IMainMenuItemViewModel>()
                            });
                        });

                        entry.Value.IfIs((MultiManagedCommand c) =>
                        {
                            var subCommands = c.SubCommands();
                            foreach(var subCommand in subCommands)
                            {
                                MetadataAsserter.AssertMetadataCollectionProperties(subCommand, "Anonymous");
                                MetadataProcessor.ProcessMetadata(subCommand);
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
                    abstractMenuPaths = CommandManager.ManagedCommands.Select(c => GetMenuMetadata<MenuPath>(c).ParentPath)
                                .Concat(CommandManager.MultiManagedCommands.Select(c => GetMultiMenuMetadata<MenuPath>(c).ParentPath)).
                             SelectMany(path => path.GetPathsToRoot()).Distinct();
                }
                return abstractMenuPaths;
            }
        }

        private TMetadata GetMenuMetadata<TMetadata>(IManagedCommand managedCommand) where TMetadata : IMenuMetadata
        {
            return managedCommand.MainMenuMetadata.OfType<TMetadata>().SingleOrDefault();
        }

        private TMetadata GetMultiMenuMetadata<TMetadata>(IMultiManagedCommand multiManagedCommand) where TMetadata : IMultiMenuMetadata
        {
            return multiManagedCommand.MenuMetadata.OfType<TMetadata>().SingleOrDefault();
        }

        private TMetadata GetSubmenuMetadata<TMetadata>(ISubCommand subCommand) where TMetadata : ISubMenuMetadata
        {
            return subCommand.SubCommandMetadata.OfType<TMetadata>().SingleOrDefault();
        }

        private void SubscribeToMultiCommandChildrenAutoInvalidationEvents(MainMenuItemViewModel viewModelItem)
        {
            var abstractMenuPath = viewModelItem.MenuEntry as AbstractMenuPath;

            if (abstractMenuPath == null){
                return;
            }

            var childMultiCommandsMetadata = CommandManager.MultiManagedCommands.Where(c => GetMultiMenuMetadata<MenuPath>(c).ParentPath == abstractMenuPath)
                                                                           .SelectMany(c => c.MenuMetadata);
            foreach(var metadata in childMultiCommandsMetadata)
            {
                metadata.IfIs((AutoInvalidateOnEvent e) => EventAggregator.Subscribe(e.EventType, () => viewModelItem.RaiseChildrenChanged(), ThreadOption.UIThread, true));
                metadata.IfIs((AutoInvalidateOnSelection s) => EventAggregator.Subscribe(s.SelectionType, () => viewModelItem.RaiseChildrenChanged(), ThreadOption.UIThread, true));
            }
        }
        
        #endregion Utils


    }
}

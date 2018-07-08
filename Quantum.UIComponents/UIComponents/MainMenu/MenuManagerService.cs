using Quantum.Command;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Quantum.UIComponents
{
    internal interface IMenuManagerService
    {
        void CreateMainMenu();
    }
    
    internal class MenuManagerService : QuantumServiceBase, IMenuManagerService
    {
        [Service]
        public ShellView ShellView { get; set; }
        
        [Service]
        public ICommandManagerService CommandManager { get; set; }

        [Service]
        public ICommandMetadataProcessorService MetadataProcessor { get; set; }


        public MenuManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public Menu MainMenu { get { return ShellView.MainMenu; } }


        public void CreateMainMenu()
        {
            var managedCommands = CommandManager.ManagedCommands.Where(c => c.MainMenuMetadata.Any());
            var multiManagedCommands = CommandManager.MultiManagedCommands.Where(c => c.MenuMetadata.Any());
            var menuPaths = managedCommands.Select(o => o.MainMenuMetadata.OfType<MenuPath>().Single()).
                     Concat(multiManagedCommands.Select(c => c.MenuMetadata.OfType<MenuPath>().Single()));
            CreateMenuSkeleton(menuPaths);
            CreateMenuOptions(managedCommands);
            CreateMultiMenuOptions(multiManagedCommands);
        }


        private readonly Dictionary<AbstractMenuPath, MenuItem> PathMenuItems = new Dictionary<AbstractMenuPath, MenuItem>();
        private readonly Dictionary<AbstractMenuPath, List<MenuEntity>> ChildMenuItems = new Dictionary<AbstractMenuPath, List<MenuEntity>>();

        #region AbstractMenuItems

        private void CreateMenuSkeleton(IEnumerable<MenuPath> menuPaths)
        {
            var abstractMenuPaths = menuPaths.Select(menuPath => menuPath.ParentPath).SelectMany(abstractMenuPath => abstractMenuPath.GetPathsToRoot()).Distinct();

            var maincategoryPaths = abstractMenuPaths.Where(path => path.ParentPath == AbstractMenuPath.Root);
            var subCategoryPaths = abstractMenuPaths.Except(maincategoryPaths);

            foreach (var mainCategoryPath in maincategoryPaths.OrderBy(path => path.OrderIndex))
            {
                var menuItem = CreateMenuItemFromAbstractPath(mainCategoryPath);
                MainMenu.Items.Add(menuItem);
                PathMenuItems.Add(mainCategoryPath, menuItem);
                ChildMenuItems.Add(mainCategoryPath, new List<MenuEntity>());
            }
            
            foreach(var depthGroup in subCategoryPaths.GroupBy(o => o.Depth).OrderBy(o => o.Key))
            {
                var groupByParent = depthGroup.GroupBy(grp => grp.ParentPath);
                foreach (var group in groupByParent)
                {
                    foreach(var categoryGroup in group.GroupBy(o => o.CategoryIndex).OrderBy(o => o.Key))
                    {
                        if(PathMenuItems[group.Key].Items.Count > 0) {
                            PathMenuItems[group.Key].Items.Add(new Separator());
                        }
                        foreach(var entry in categoryGroup.OrderBy(g => g.OrderIndex)) {
                            var menuItem = CreateMenuItemFromAbstractPath(entry);
                            PathMenuItems[group.Key].Items.Add(menuItem);
                            PathMenuItems.Add(entry, menuItem);
                            ChildMenuItems[group.Key].Add(new MenuEntity(entry, menuItem));
                            ChildMenuItems.Add(entry, new List<MenuEntity>());
                        }
                    }
                }
            }
        }
        
        private MenuItem CreateMenuItemFromAbstractPath(AbstractMenuPath abstractMenuPath)
        {
            var menuItem = new MenuItem();
            menuItem.Header = abstractMenuPath.Description.IfNotNull(d => d.Value, "Menu Entry");
            if(abstractMenuPath.Icon != null) {
                menuItem.Icon = IconUtils.GetResourceIcon(abstractMenuPath.Icon.IconPath);
            }
            return menuItem;
        }

        #endregion AbstractMenuItems

        #region MenuOptions

        private void CreateMenuOptions(IEnumerable<ManagedCommand> commands)
        {
            foreach(var command in commands) {
                var menuPath = command.MainMenuMetadata.OfType<MenuPath>().Single();
                var menuItem = CreateMenuOptionFromCommand(command);

                var parentMenuItem = PathMenuItems[menuPath.ParentPath];
                var neighbours = ChildMenuItems[menuPath.ParentPath].OrderBy(o => o.MenuEntry.CategoryIndex).ThenBy(o => o.MenuEntry.OrderIndex);
                
                
                if(!neighbours.Any()) 
                {
                    parentMenuItem.Items.Add(menuItem);
                    
                }
                else if(neighbours.Any(path => path.MenuEntry.CategoryIndex == menuPath.CategoryIndex)) 
                {
                    var categoryNeighbours = neighbours.Where(n => n.MenuEntry.CategoryIndex == menuPath.CategoryIndex);
                    if(!categoryNeighbours.Any(o => o.MenuEntry.OrderIndex > menuPath.OrderIndex)) {
                        InsertAfter(parentMenuItem, categoryNeighbours.Last().MenuItem, menuItem);
                    }
                    else if (!categoryNeighbours.Any(o => o.MenuEntry.OrderIndex < menuPath.OrderIndex)) {
                        InsertBefore(parentMenuItem, categoryNeighbours.First().MenuItem, menuItem);
                    }
                    else {
                        InsertBefore(parentMenuItem, categoryNeighbours.First(o => o.MenuEntry.OrderIndex > menuPath.OrderIndex).MenuItem, menuItem);
                    }
                }
                else if(!neighbours.Any(path => path.MenuEntry.CategoryIndex > menuPath.CategoryIndex))
                {
                    parentMenuItem.Items.Add(new Separator());
                    parentMenuItem.Items.Add(menuItem);
                }
                else if(!neighbours.Any(path => path.MenuEntry.CategoryIndex < menuPath.CategoryIndex))
                {
                    parentMenuItem.Items.Insert(0, new Separator());
                    parentMenuItem.Items.Insert(0, menuItem);
                }
                else
                {
                    InsertBefore(parentMenuItem, neighbours.First(o => o.MenuEntry.CategoryIndex > menuPath.CategoryIndex).MenuItem, menuItem, true);
                }
                ChildMenuItems[menuPath.ParentPath].Add(new MenuEntity(menuPath, menuItem));
            }
        }

        private MenuItem CreateMenuOptionFromCommand(ManagedCommand command)
        {
            var menuItem = new MenuItem();

            var checkable = command.MainMenuMetadata.OfType<Checkable>().SingleOrDefault();
            var checkChanged = command.MainMenuMetadata.OfType<CheckChanged>().SingleOrDefault();
            var description = command.MainMenuMetadata.OfType<Description>().SingleOrDefault();
            var icon = command.MainMenuMetadata.OfType<Icon>().SingleOrDefault();
            var keyShortcut = command.MainMenuMetadata.OfType<KeyShortcut>().SingleOrDefault();
            var toolTip = command.MainMenuMetadata.OfType<Command.ToolTip>().SingleOrDefault();

            checkable.IfNotNull(metadata => menuItem.IsCheckable = metadata.Value);
            checkChanged.IfNotNull(metadata => { menuItem.Checked += (sender, e) => metadata.OnCheckChanged(true); menuItem.Unchecked += (sender, e) => metadata.OnCheckChanged(false); });
            description.IfNotNull(metadata => menuItem.Header = metadata.Value);
            icon.IfNotNull(metadata => menuItem.Icon = IconUtils.GetResourceIcon(metadata.IconPath));
            keyShortcut.IfNotNull(metadata =>
            {
                ShellView.InputBindings.Add(new KeyBinding(command, new KeyGesture(metadata.Key, metadata.ModifierKeys)));
                menuItem.InputGestureText = metadata.GetInputGestureText();
            });
            toolTip.IfNotNull(metadata => menuItem.ToolTip = metadata.Value);
            menuItem.Command = command;
            

            return menuItem;
        }

        private void InsertBefore(MenuItem parent, MenuItem child, MenuItem newItem, bool addSeparator = false)
        {
            var index = parent.Items.IndexOf(child);
            if(addSeparator)
            {
                parent.Items.Insert(index, new Separator());
            }
            parent.Items.Insert(index, newItem);
        }

        private void InsertAfter(MenuItem parent, MenuItem child, MenuItem newItem, bool addSeparator = false)
        {
            var index = parent.Items.IndexOf(child) + 1;
            parent.Items.Insert(index, newItem);
            if (addSeparator)
            {
                parent.Items.Insert(index, new Separator());
            }
        }

        #endregion MenuOptions

        #region MultiMenuOptions

        private readonly Dictionary<MultiManagedCommand, IEnumerable<MenuItem>> RegisteredMultiOptions = new Dictionary<MultiManagedCommand, IEnumerable<MenuItem>>();  

        private void CreateMultiMenuOptions(IEnumerable<MultiManagedCommand> multiCommands)
        {
            //TODO : Implement.
        }

        #endregion MultiMenuOptions
    }

    internal class MenuEntity
    {
        public IMenuEntry MenuEntry { get; private set; }
        public MenuItem MenuItem { get; private set; }

        public MenuEntity(IMenuEntry menuEntry, MenuItem menuItem)
        {
            MenuEntry = menuEntry;
            MenuItem = menuItem;
        }
    }
}

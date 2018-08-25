using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Quantum.UIComponents
{
    internal static class AvalonContextMenuProperties
    {
        #region ContextMenu

        #region HideEmptySeparatorGroups

        public static bool GetHideEmptySeparatorGroups(DependencyObject obj)
        {
            return (bool)obj.CheckedGetValue(HideEmptySeparatorGroupsProperty);
        }

        public static void SetHideEmptySeparatorGroups(DependencyObject obj, bool value)
        {
            obj.CheckedSetValue(HideEmptySeparatorGroupsProperty, value);
        }

        // Using a DependencyProperty as the backing store for HideEmptySeparatorGroups.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HideEmptySeparatorGroupsProperty =
            DependencyProperty.RegisterAttached("HideEmptySeparatorGroups", typeof(bool), typeof(AvalonContextMenuProperties), new PropertyMetadata(true));

        #endregion

        #region HideInvisibleMenu

        public static bool GetHideInvisibleMenu(DependencyObject obj)
        {
            return (bool)obj.CheckedGetValue(HideInvisibleMenuProperty);
        }

        public static void SetHideInvisibleMenu(DependencyObject obj, bool value)
        {
            obj.CheckedSetValue(HideInvisibleMenuProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanShowMenu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HideInvisibleMenuProperty =
            DependencyProperty.RegisterAttached(
            "HideInvisibleMenu",
            typeof(bool),
            typeof(AvalonContextMenuProperties),
            new UIPropertyMetadata(HideInvisibleMenuChanged));

        [SuppressMessage("Microsoft.Defisn", "IDE0019")]
        public static void HideInvisibleMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = d as ItemsControl;
            var menuItem = d as MenuItem;
            var menu = d as ContextMenu;

            if (itemsControl == null)
            {
                throw new ArgumentException("The property HideInvisibleMenu can only be set on a ContextMenu control !");
            }
            if ((bool)e.NewValue)
            {
                itemsControl.Loaded += menu_Loaded;
                if (menu != null)
                {
                    menu.Opened += menu_Opened;
                }
                if (menuItem != null) menuItem.SubmenuOpened += menu_Opened;
            }
            else
            {
                itemsControl.Loaded -= menu_Loaded;
                if (menu != null) menu.Opened -= menu_Opened;
                if (menuItem != null) menuItem.SubmenuOpened -= menu_Opened;
            }
        }

        [SuppressMessage("Microsoft.Design", "IDE1006")]
        static void menu_Opened(object sender, RoutedEventArgs e)
        {
            menu_Loaded(sender, e);
        }

        [SuppressMessage("Microsoft.Design", "IDE1006")]
        [SuppressMessage("Microsoft.Design", "IDE0018")]
        [SuppressMessage("Microsoft.Design", "IDE0019")]
        static void menu_Loaded(object sender, RoutedEventArgs e)
        {
            var menu = (e == null ? sender : e.OriginalSource).SafeCast<ItemsControl>("Could not find the {0}. The property HideInvisibleMenu should be attached to an items control");

            if (!ReferenceEquals(sender, menu)) return;

            var menuItem = menu as MenuItem;

            if (menu.Items.Count == 0 && (menuItem != null && (menuItem.Command != null || menuItem.IsCheckable)))
            {
                SetHasVisibleMenuItems(menu, true);
                return;
            }
            bool visible = false;

            var itemGen = (IItemContainerGenerator)menu.ItemContainerGenerator;
            using (itemGen.StartAt(itemGen.GeneratorPositionFromIndex(0), GeneratorDirection.Forward, true))
            {
                bool lastGroupHasItems = false;
                bool groupMustStayCollapsed = false;
                Separator lastSeparator = null;
                for (int i = 0; i < menu.Items.Count; i++)
                {
                    bool isNew;
                    var genChild = itemGen.GenerateNext(out isNew);
                    var item = genChild as MenuItem;
                    var currentSeparator = genChild as Separator;

                    if (item != null)
                    {
                        if (GetHideInvisibleMenu(item))
                        {
                            menu_Loaded(item, null);
                        }

                        if (item.IsEnabled && item.Visibility == Visibility.Visible)
                        {
                            if ((item.Command == null && GetHasVisibleMenuItems(item))
                               || (item.Command != null && item.Command.CanExecute(item.CommandParameter))) visible = true;
                            // We do not hide cheackbe items. If they have a command then that takes precedance, 
                            // as there are commands that have state (IStatefulManagedCommands) that should be hidden if they cannot be exected
                            if (item.IsCheckable && item.Command == null) visible = true;
                            lastGroupHasItems = true;
                        }
                    }
                    else
                    {
                        if (lastSeparator != null)
                        {
                            lastSeparator.Visibility = lastGroupHasItems && !groupMustStayCollapsed ? Visibility.Visible : Visibility.Collapsed;
                            groupMustStayCollapsed = !lastGroupHasItems && groupMustStayCollapsed;
                        }
                        else if (!lastGroupHasItems)
                        {
                            currentSeparator.IfNotNull(_ => _.Visibility = Visibility.Collapsed);
                            groupMustStayCollapsed = true;
                        }
                        lastSeparator = currentSeparator;
                        lastGroupHasItems = false;
                    }
                }

                if (lastSeparator != null)
                {
                    lastSeparator.Visibility = lastGroupHasItems && !groupMustStayCollapsed ? Visibility.Visible : Visibility.Collapsed;
                }

            }

            SetHasVisibleMenuItems(menu, visible);
            if (menu is ContextMenu && !visible)
            {
                ((ContextMenu)menu).IsOpen = false;
            }
        }

        #endregion

        #region HasVisibleMenuItems

        public static bool GetHasVisibleMenuItems(DependencyObject obj)
        {
            return (bool)obj.CheckedGetValue(HasVisibleMenuItemsProperty);
        }

        public static void SetHasVisibleMenuItems(DependencyObject obj, bool value)
        {
            obj.CheckedSetValue(HasVisibleMenuItemsProperty, value);
        }

        // Using a DependencyProperty as the backing store for HasVisibleMenuItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasVisibleMenuItemsProperty =
            DependencyProperty.RegisterAttached("HasVisibleMenuItems", typeof(bool), typeof(AvalonContextMenuProperties), new UIPropertyMetadata(true));

        #endregion

        #endregion

        #region MenuItem
        public static bool GetHideInvisibleMenuItem(DependencyObject obj)
        {
            return (bool)obj.CheckedGetValue(HideInvisibleMenuItemProperty);
        }

        public static void SetHideInvisibleMenuItem(DependencyObject obj, bool value)
        {
            obj.CheckedSetValue(HideInvisibleMenuItemProperty, value);
        }

        // Using a DependencyProperty as the backing store for HideInvisibleMenuItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HideInvisibleMenuItemProperty =
            DependencyProperty.RegisterAttached(
            "HideInvisibleMenuItem",
            typeof(bool),
            typeof(AvalonContextMenuProperties),
            new UIPropertyMetadata(HideInvisibleMenuItemChanged));

        [SuppressMessage("Microsoft.Design", "IDE0019")]
        public static void HideInvisibleMenuItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = d as MenuItem;

            if (menu == null)
            {
                throw new ArgumentException("The property HideInvisibleMenuItem can only be set on a ContextMenu control !");
            }
            if ((bool)e.NewValue)
            {
                menu.Loaded += menuItem_Loaded;
            }
            else
            {
                menu.Loaded -= menuItem_Loaded;
            }
        }

        [SuppressMessage("Microsoft.Design", "IDE1006")]
        [SuppressMessage("Microsoft.Design", "IDE0019")]
        static void menuItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ReferenceEquals(sender, e.OriginalSource)) return;

            var menu = e.OriginalSource as MenuItem;

            if (menu == null) return;

            bool visible = false;

            foreach (MenuItem item in menu.Items.OfType<MenuItem>())
            {
                if (item.IsEnabled) visible = true;
            }

            if (visible)
            {
                menu.Visibility = Visibility.Visible;
            }
            else
            {
                menu.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region ContextMenuSizeBugFix

        public static bool GetContextMenuSizeBugFix(DependencyObject obj)
        {
            return (bool)obj.CheckedGetValue(ContextMenuSizeBugFixProperty);
        }

        public static void SetContextMenuSizeBugFix(DependencyObject obj, bool value)
        {
            obj.CheckedSetValue(ContextMenuSizeBugFixProperty, value);
        }

        // Fix for a known WPF resizing issue. Sometimes when a context menu is displayed, if menu item visibility is changed, the size 
        // of the context menu shrinks to a very small value (~30px x ~20px) to fix this issue when the menu is loaded be must invalidate the measure
        // for the root visual in the context menu. To enable this invalidate, this property must be set to true on the root element of the context menu's template
        // Do no use this hack unless you are experiencing the problem described above.
        public static readonly DependencyProperty ContextMenuSizeBugFixProperty =
            DependencyProperty.RegisterAttached("ContextMenuSizeBugFix", typeof(bool), typeof(AvalonContextMenuProperties), new PropertyMetadata(false,
               (d, e) =>
               {
                   var uiElement = (System.Windows.FrameworkElement)d;
                   if ((bool)e.OldValue)
                   {
                       uiElement.Loaded -= uiElement_Loaded;
                   }

                   if ((bool)e.NewValue)
                   {
                       uiElement.Loaded += uiElement_Loaded;
                   }
               }));

        [SuppressMessage("Microsoft.Design", "IDE1006")]
        static void uiElement_Loaded(object sender, RoutedEventArgs e)
        {
            ((UIElement)sender).InvalidateMeasure();
        }

        #endregion

    }
}

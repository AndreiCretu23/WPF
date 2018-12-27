using Quantum.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UIItemsControl = System.Windows.Controls.ItemsControl;

namespace Quantum.Controls
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(TreeViewItem))]
    public class TreeView : UIItemsControl
    {
        #region DependencyProperties

        public static readonly DependencyProperty AllowMultipleSelectionProperty = DependencyProperty.Register
        (
            name: "AllowMultipleSelection",
            propertyType: typeof(bool),
            ownerType: typeof(TreeView),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty AllowTabNavigationProperty = DependencyProperty.Register
        (
            name: "AllowTabNavigation",
            propertyType: typeof(bool),
            ownerType: typeof(TreeView),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty AllowArrowNavigationProperty = DependencyProperty.Register
        (
            name: "AllowArrowNavigation",
            propertyType: typeof(bool),
            ownerType: typeof(TreeView),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );
        
        #endregion DependencyProperties

        #region Properties

        public bool AllowMultipleSelection
        {
            get { return (bool)GetValue(AllowMultipleSelectionProperty); }
            set { SetValue(AllowMultipleSelectionProperty, value); }
        }

        public bool AllowTabNavigation
        {
            get { return (bool)GetValue(AllowTabNavigationProperty); }
            set { SetValue(AllowTabNavigationProperty, value); }
        }

        public bool AllowArrowNavigation
        {
            get { return (bool)GetValue(AllowArrowNavigationProperty); }
            set { SetValue(AllowArrowNavigationProperty, value); }
        }

        #endregion Properties

        internal TreeViewSelectionManager SelectionManager { get; }
        internal TreeViewNavigationManager NavigationManager { get; }

        public TreeView()
        {
            SelectionManager = new TreeViewSelectionManager(this);
            NavigationManager = new TreeViewNavigationManager(this);
        }

        static TreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeView), new FrameworkPropertyMetadata(typeof(TreeView)));
        }

        #region ItemContainerConfig

        protected override bool ShouldApplyItemContainerStyle(DependencyObject container, object item)
        {
            return container is TreeViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItem();
        }

        #endregion ItemContainerConfig
        
        #region Keyboard

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if(e.KeyboardDevice.Modifiers == ModifierKeys.Shift && e.KeyboardDevice.IsKeyDown(Key.Tab)) {
                NavigationManager.HandleBackTabNavigation();
            }

            else if(e.KeyboardDevice.Modifiers == ModifierKeys.Shift && e.KeyboardDevice.IsKeyDown(Key.Up)) {
                NavigationManager.HandleMultipleBackArrowNavigation();
            }

            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift && e.KeyboardDevice.IsKeyDown(Key.Down)) {
                NavigationManager.HandleMultipleArrowNavigation();
            }

            else if (e.KeyboardDevice.IsKeyDown(Key.Tab)) {
                NavigationManager.HandleTabNavigation();
            }

            else if (e.KeyboardDevice.IsKeyDown(Key.Up)) {
                NavigationManager.HandleBackArrowNavigation();
            }

            else if (e.KeyboardDevice.IsKeyDown(Key.Down)) {
                NavigationManager.HandleArrowNavigation();
            }

            else {
                base.OnKeyDown(e);
                return;
            }

            e.Handled = true;
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if((e.Key == Key.LeftShift || e.Key == Key.RightShift)) {
                NavigationManager.TeardownMultipleArrowNavigationSession();
            }
            base.OnPreviewKeyUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.KeyboardDevice.IsKeyDown(Key.A)) {
                SelectionManager.SelectAllItems();
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }

        #endregion Keyboard

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Quantum.Utils;

namespace Quantum.Controls
{
    public class TreeViewItem : System.Windows.Controls.ItemsControl
    {
        #region DependencyProperties

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register
        (
            name: "IsExpanded",
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        #endregion DependencyProperties

        #region Properties

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        #endregion Properties


        public TreeViewItem()
        {
        }

        static TreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(typeof(TreeViewItem)));
        }

        protected override bool ShouldApplyItemContainerStyle(DependencyObject container, object item)
        {
            return container is TreeViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItem();
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Quantum.Controls
{
    public class TreeView : System.Windows.Controls.ItemsControl
    {
        static TreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeView), new FrameworkPropertyMetadata(typeof(TreeView)));
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

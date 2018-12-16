using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Quantum.Controls
{
    public class TreeViewItem : System.Windows.Controls.ItemsControl
    {
        static TreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(typeof(TreeViewItem)));
        }
    }
}

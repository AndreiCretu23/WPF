using System;
using System.Collections.Generic;
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
    }
}

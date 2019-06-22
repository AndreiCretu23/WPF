using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF.Panels
{
    [Guid("782F5311-4056-45DE-A750-E70D9F989D2F")]
    public partial class TreePanelView : UserControl, ITreePanelView
    {
        public TreePanelView()
        {
            InitializeComponent();
        }
    }
}

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
    /// <summary>
    /// Interaction logic for ActivePanelView.xaml
    /// </summary>
    [Guid("B34A1C30-508D-4738-83F8-81C49BCD76F5")]
    public partial class ActivePanelView : UserControl, IActivePanelView
    {
        public ActivePanelView()
        {
            InitializeComponent();
        }
    }
}

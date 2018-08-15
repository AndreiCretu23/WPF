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
    /// Interaction logic for SelectionPanelView.xaml
    /// </summary>
    [Guid("4918F066-77D8-4DBA-B7CA-82513BE48883")]
    public partial class SelectionPanelView : UserControl, ISelectionPanelView
    {
        public SelectionPanelView()
        {
            InitializeComponent();
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace WPF.Panels
{
    [Guid("3EB3CE96-D45B-4AC4-8322-6AA442A14902")]
    public partial class ListPanelView : UserControl, IListPanelView
    {
        public ListPanelView()
        {
            InitializeComponent();
        }
    }
}

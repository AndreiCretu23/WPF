using System;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace WPF.Panels
{
    [Guid("007581E6-3445-45B5-8DBE-438392F3C604")]
    public partial class SourcePanelView : UserControl, ISourcePanelView
    {
        public SourcePanelView()
        {
            InitializeComponent();
        }
    }
}

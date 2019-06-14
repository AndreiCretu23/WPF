using Quantum.Command;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

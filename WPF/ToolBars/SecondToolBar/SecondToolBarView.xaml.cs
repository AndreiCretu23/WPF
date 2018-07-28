using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace WPF.ToolBars
{
    [Guid("8A99371D-CF3B-4972-88D9-855A5213DB98")]
    public partial class SecondToolBarView : UserControl, ISecondToolBarView
    {
        public SecondToolBarView()
        {
            InitializeComponent();
        }
    }

    [Guid("FEFA27C5-FF87-419F-9B8C-7544055E644A")]
    public interface ISecondToolBarView
    {
    }
}

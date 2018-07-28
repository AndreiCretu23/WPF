using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace WPF.ToolBars
{
    [Guid("7FAED49A-3D07-4DA8-94AA-5704B29DDC05")]
    public partial class FirstToolBarView : UserControl, IFirstToolBarView
    {
        public FirstToolBarView()
        {
            InitializeComponent();
        }
    }

    [Guid("7D7D94DF-0795-4FC7-A421-BE9A9713DB2D")]
    public interface IFirstToolBarView
    {
    }
}

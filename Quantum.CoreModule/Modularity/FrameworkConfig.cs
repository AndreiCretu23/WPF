using Quantum.ResourceLibrary;
using Quantum.Services;
using Quantum.Utils;
using System.Windows;

namespace Quantum.CoreModule
{
    /// <summary>
    /// A selection, registered in the container, that is bound to the text displayed 
    /// in the center of the loading animation that pops up when the UIThread is busy.<para></para>
    /// Default value is : "Loading..."
    /// </summary>
    public class SelectedLongOperationDescription : SingleSelection<string>
    {
        public SelectedLongOperationDescription()
            : base(Resources.LongOperation_DefaultDescription, false)
        {
        }
    }

    /// <summary>
    /// A selection, registered in the container, bound to the title
    /// of the MainWindow of the application. <para></para>
    /// Default value is the application's name.
    /// </summary>
    public class SelectedShellTitle : SingleSelection<string>
    {
        public SelectedShellTitle()
            : base(AppInfo.ApplicationName, false)
        {
        }
    }

    /// <summary>
    /// A selection, registered in the container, bound to the icon 
    /// of the MainWindow of the application. <para></para>
    /// Default value is : null (none).
    /// </summary>
    public class SelectedShellIcon : SingleSelection<string>
    {
    }

    /// <summary>
    /// A selection, registered in the container, bound to the resize mode 
    /// of the MainWindow of the application. <para></para>
    /// DefaultValue is : CanResizeWithGrip.
    /// </summary>
    public class SelectedShellResizeMode : SingleSelection<ResizeMode>
    {
        public SelectedShellResizeMode()
            : base(ResizeMode.CanResizeWithGrip, false)
        {
        }
    }
}

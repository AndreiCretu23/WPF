using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Services;
using System.Windows;
using Quantum.Utils;
using System.Threading;
using Microsoft.Practices.Unity;

namespace WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var bootStrapper = new Bootstrapper();
            bootStrapper.Run();
            

            base.OnStartup(e);
        }
        
    }
}

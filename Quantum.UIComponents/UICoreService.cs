using Quantum.Services;
using Quantum.UIComponents.Shell;
using Quantum.Utils;
using System.Windows;
using Unity;

namespace Quantum.UIComponents
{
    public interface IUICoreService
    {
        /// <summary>
        /// Creates, configures and displays the application's main view.
        /// </summary>
        void CreateUI();
    }

    internal class UICoreService : QuantumServiceBase, IUICoreService
    {
        public UICoreService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        
        public void CreateUI()
        {
            var shellViewModel = Container.Resolve<ShellViewModel>();
            var shellView = Container.Resolve<ShellView>();

            shellView.DataContext = shellViewModel;
            shellView.Title = AppInfo.ApplicationName;

            Application.Current.MainWindow = shellView;

            shellView.Show();
        }
    }
}

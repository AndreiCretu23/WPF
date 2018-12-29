using Quantum.CoreModule;
using Quantum.Events;
using Quantum.Services;
using Quantum.Utils;
using System.Windows;

namespace Quantum.UIComponents
{
    public interface IUICoreService
    {
        /// <summary>
        /// Creates, configures and displays the application's main view.
        /// </summary>
        void CreateUI();
    }

    internal class UICoreService : ServiceBase, IUICoreService
    {
        [Service]
        public ShellView ShellView { get; set; }

        [Service]
        public ShellViewModel ShellViewModel { get; set; }
        
        public UICoreService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        
        public void CreateUI()
        {
            void onShellLoaded(object sender, RoutedEventArgs e)
            {
                EventAggregator.GetEvent<UILoadedEvent>().Publish(new UILoadedArgs());
                ShellView.Loaded -= onShellLoaded;
            }
            ShellView.Loaded += onShellLoaded;

            ShellView.DataContext = ShellViewModel;
            ShellView.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ShellView.WindowState = WindowState.Maximized;
            Application.Current.MainWindow = ShellView;
            
            ShellView.Show();
        }
    }
}

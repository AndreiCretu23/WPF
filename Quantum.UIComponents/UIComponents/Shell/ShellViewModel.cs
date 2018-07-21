using Quantum.Services;
using Quantum.Command;
using System.Windows;
using System.ComponentModel;
using Microsoft.Practices.Composite.Presentation.Commands;
using System.Windows.Input;

namespace Quantum.UIComponents
{
    public class ShellViewModel : ViewModelBase
    {
        public IMainMenuViewModel MainMenuViewModel { get; set; }

        public ShellViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            MainMenuViewModel = new MainMenuViewModel(initSvc);
        }
        
    }
}

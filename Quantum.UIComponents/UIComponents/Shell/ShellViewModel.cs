using Quantum.Services;
using Quantum.Command;
using System.Windows;
using System.ComponentModel;
using Microsoft.Practices.Composite.Presentation.Commands;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.UIComponents
{
    public class ShellViewModel : ViewModelBase
    {
        [Service]
        public ICommandManagerService CommandManager { get; set; }


        public IEnumerable<KeyBinding> Shortcuts
        {
            get
            {
                var commands = CommandManager.ManagedCommands.Where(c => c.MainMenuMetadata.OfType<KeyShortcut>().Count() == 1);
                foreach(var command in commands)
                {
                    var shortcut = command.MainMenuMetadata.OfType<KeyShortcut>().Single();
                    yield return new KeyBinding(command, new KeyGesture(shortcut.Key, shortcut.ModifierKeys));
                }
            }
        }
        
        public IMainMenuViewModel MainMenuViewModel { get; set; }

        public ShellViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            MainMenuViewModel = new MainMenuViewModel(initSvc);
        }
        
    }
}

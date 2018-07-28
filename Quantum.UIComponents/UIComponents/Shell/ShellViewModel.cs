using Quantum.Command;
using Quantum.Metadata;
using Quantum.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

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
        public IToolBarContainerViewModel ToolBarContainerViewModel { get; set; }

        public ShellViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            MainMenuViewModel = new MainMenuViewModel(initSvc);
            ToolBarContainerViewModel = new ToolBarContainerViewModel(initSvc);
        }
        
    }
}

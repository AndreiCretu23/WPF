using Quantum.Command;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Quantum.UIComponents
{
    internal class ShellShortcutsViewModel : ViewModelBase, IShellShortcutsViewModel
    {
        [Service]
        public ICommandManagerService CommandManager { get; set; }

        [Service]
        public IPanelManagerService PanelManager { get; set; }

        public IEnumerable<KeyBinding> Shortcuts { get { return GetCommandShortcuts().Concat(GetPanelBringIntoViewShortcuts()); } }

        public ShellShortcutsViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        
        private IEnumerable<KeyBinding> GetCommandShortcuts()
        {
            var managedCommands = CommandManager.ManagedCommands.Where(c => c.Metadata.OfType<KeyShortcut>().Any());
            foreach(var command in managedCommands) {
                var shortcut = command.Metadata.OfType<KeyShortcut>().Single();
                yield return new KeyBinding(command, new KeyGesture(shortcut.Key, shortcut.ModifierKeys));
            }
        }

        public IEnumerable<KeyBinding> GetPanelBringIntoViewShortcuts()
        {
            var panelDefinitions = PanelManager.StaticPanelDefinitions.Where(def => def.OfType<BringIntoViewOnKeyShortcut>().Any());
            foreach(var definition in panelDefinitions) {
                var shortcut = definition.OfType<BringIntoViewOnKeyShortcut>().Single();
                var bringIntoViewCommand = new DelegateCommand()
                {
                    CanExecuteHandler = () => definition.OfType<StaticPanelConfiguration>().Single().CanOpen(),
                    ExecuteHandler = () => typeof(IPanelManagerService).GetMethod(nameof(PanelManager.BringStaticPanelIntoView)).MakeGenericMethod(definition.IViewModel).Invoke(PanelManager, new object[] { }),
                };

                yield return new KeyBinding(bringIntoViewCommand, new KeyGesture(shortcut.Key, shortcut.ModifierKeys));
            }
        }
    }
}

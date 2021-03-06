﻿using Quantum.Command;
using Quantum.Events;
using Quantum.Metadata;
using Quantum.Services;
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

        private IEnumerable<KeyBinding> GlobalCommandShortcuts { get; set; }
        private IEnumerable<KeyBinding> BringPanelIntoViewShortcuts { get; set; }

        public IEnumerable<KeyBinding> Shortcuts { get { return GlobalCommandShortcuts.Concat(BringPanelIntoViewShortcuts); } }

        public ShellShortcutsViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            GlobalCommandShortcuts = GetCommandShortcuts();
            BringPanelIntoViewShortcuts = GetBringPanelIntoViewShortcuts();
        }
        
        private IEnumerable<KeyBinding> GetCommandShortcuts()
        {
            var globalCommands = CommandManager.GlobalCommands.Where(c => c.Metadata.OfType<KeyShortcut>().Any());
            foreach(var command in globalCommands) {
                var shortcut = command.Metadata.OfType<KeyShortcut>().Single();
                yield return new KeyBinding(command, new KeyGesture(shortcut.Key, shortcut.ModifierKeys));
            }
        }

        public IEnumerable<KeyBinding> GetBringPanelIntoViewShortcuts()
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
        
        [Handles(typeof(ShortcutChangedEvent))]
        public void OnShortcutChanged(IShortcutChangedArgs args)
        {
            if(args is GlobalCommandShortcutChangedArgs) {
                GlobalCommandShortcuts = GetCommandShortcuts();
            }
            else if(args is BringPanelIntoViewShortcutChangedArgs) {
                BringPanelIntoViewShortcuts = GetBringPanelIntoViewShortcuts();
            }
            else if(args is GlobalRebuildShortcutChangedArgs) {
                GlobalCommandShortcuts = GetCommandShortcuts();
                BringPanelIntoViewShortcuts = GetBringPanelIntoViewShortcuts();
            }

            RaisePropertyChanged(() => Shortcuts);
        }
    }
}

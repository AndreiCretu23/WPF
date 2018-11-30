﻿using Quantum.Command;
using Quantum.CoreModule;
using Quantum.Metadata;
using Quantum.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Quantum.UIComponents
{
    internal class ShellViewModel : ViewModelBase
    {
        [Service]
        public ICommandManagerService CommandManager { get; set; }

        [Selection]
        public SelectedShellTitle SelectedTitle { get; set; }

        [Selection]
        public SelectedShellIcon SelectedIcon { get; set; }

        [Selection]
        public SelectedShellResizeMode SelectedResizeMode { get; set; }

        #region Config

        [InvalidateOn(typeof(SelectedShellTitle))]
        public string Title { get { return SelectedTitle.Value; } }

        [InvalidateOn(typeof(SelectedShellIcon))]
        public string Icon { get { return SelectedIcon.Value; } }

        [InvalidateOn(typeof(SelectedShellResizeMode))]
        public ResizeMode ResizeMode { get { return SelectedResizeMode.Value; } }
        
        #endregion Config

        public IEnumerable<KeyBinding> Shortcuts
        {
            get
            {
                var commands = CommandManager.ManagedCommands.Where(c => c.Metadata.OfType<KeyShortcut>().Count() == 1);
                foreach(var command in commands)
                {
                    var shortcut = command.Metadata.OfType<KeyShortcut>().Single();
                    yield return new KeyBinding(command, new KeyGesture(shortcut.Key, shortcut.ModifierKeys));
                }
            }
        }
        
        public IMainMenuViewModel MainMenuViewModel { get; set; }
        public IToolBarContainerViewModel ToolBarContainerViewModel { get; set; }
        public IDockingView DockingView { get; set; }

        public ShellViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            MainMenuViewModel = new MainMenuViewModel(initSvc);
            ToolBarContainerViewModel = new ToolBarContainerViewModel(initSvc);
            DockingView = Container.Resolve<IDockingView>();
        }
        
    }
}

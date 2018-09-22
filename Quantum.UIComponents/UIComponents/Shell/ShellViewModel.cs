using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Command;
using Quantum.CoreModule;
using Quantum.Metadata;
using Quantum.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;

namespace Quantum.UIComponents
{
    internal class ShellViewModel : ViewModelBase
    {
        [Service]
        public ICommandManagerService CommandManager { get; set; }
        
        [Service]
        public IFrameworkConfig FrameworkConfig { get; set; }

        #region Config

        public string Title => FrameworkConfig.ShellTitle;
        public string Icon => FrameworkConfig.ShellIcon;

        public double Width => FrameworkConfig.ShellWidth;
        public double Height => FrameworkConfig.ShellHeight;

        public double MinWidth => FrameworkConfig.ShellMinWidth;
        public double MinHeight => FrameworkConfig.ShellMinHeight;

        public double MaxWidth => FrameworkConfig.ShellMaxWidth;
        public double MaxHeight => FrameworkConfig.ShellMaxHeight;

        public ResizeMode ResizeMode => FrameworkConfig.ShellResizeMode;
        public WindowState WindowState => FrameworkConfig.ShellState;

        private void ProcessConfigInvalidators()
        {
            ResolveConfigPropertyInvalidators(() => Title,           config => config.ShellTitle);
            ResolveConfigPropertyInvalidators(() => Icon,            config => config.ShellIcon);
            ResolveConfigPropertyInvalidators(() => Width,           config => config.ShellWidth);
            ResolveConfigPropertyInvalidators(() => Height,          config => config.ShellHeight);
            ResolveConfigPropertyInvalidators(() => MinWidth,        config => config.ShellMinWidth);
            ResolveConfigPropertyInvalidators(() => MinHeight,       config => config.ShellMinHeight);
            ResolveConfigPropertyInvalidators(() => MaxWidth,        config => config.ShellMaxWidth);
            ResolveConfigPropertyInvalidators(() => MaxHeight,       config => config.ShellMaxHeight);
            ResolveConfigPropertyInvalidators(() => ResizeMode,      config => config.ShellResizeMode);
            ResolveConfigPropertyInvalidators(() => WindowState,     config => config.ShellState);
        }

        private void ResolveConfigPropertyInvalidators<T>(Expression<Func<T>> property, Expression<Func<IFrameworkConfig, T>> configProperty)
        {
            var invalidators = FrameworkConfig.GetPropertyInvalidators(configProperty);
            foreach(var evt in invalidators)
            {
                EventAggregator.Subscribe(evt, () => RaisePropertyChanged(property), ThreadOption.UIThread, true);
            }
        }

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
            ProcessConfigInvalidators();
            MainMenuViewModel = new MainMenuViewModel(initSvc);
            ToolBarContainerViewModel = new ToolBarContainerViewModel(initSvc);
            DockingView = Container.Resolve<IDockingView>();
        }
        
    }
}

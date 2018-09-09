using Quantum.Command;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System.Linq;

namespace Quantum.UIComponents
{
    internal class MainMenuCommandViewModel : ViewModelBase, IMainMenuItemViewModel
    {
        #region Fields

        private bool isChecked;
        
        #endregion Fields


        public IManagedCommand Command { get; }
        private IMainMenuCommandExtractor CommandExtractor { get; }

        public string Header => CommandExtractor.GetMenuMetadata<Description>(Command)?.Value;
        public string Icon => CommandExtractor.GetMenuMetadata<Icon>(Command)?.IconPath;
        public bool IsCheckable => CommandExtractor.GetMenuMetadata<Checkable>(Command).IfNotNull(o => o.Value);
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                RaisePropertyChanged(() => IsChecked);
                CommandExtractor.GetMenuMetadata<CheckChanged>(Command)?.OnCheckChanged?.Invoke(value);
            }
        }
        public string Shortcut => Command.Metadata.OfType<KeyShortcut>().SingleOrDefault()?.GetInputGestureText();
        public string ToolTip => CommandExtractor.GetMenuMetadata<ToolTip>(Command)?.Value;

        public MainMenuCommandViewModel(IObjectInitializationService initSvc, IMainMenuCommandExtractor commandExtractor, IManagedCommand command)
            :base(initSvc)
        {
            CommandExtractor = commandExtractor;
            Command = command;
        }
        
    }
}

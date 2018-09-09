using Quantum.Command;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System.Linq;

namespace Quantum.UIComponents
{
    internal class MainMenuSubCommandViewModel : ViewModelBase, IMainMenuItemViewModel
    {
        private IMainMenuCommandExtractor CommandExtractor { get; }

        #region Fields

        private bool isChecked;

        #endregion Fields

        public ISubCommand SubCommand { get; }

        public string Header => CommandExtractor.GetSubmenuMetadata<Description>(SubCommand)?.Value;
        public string Icon => CommandExtractor.GetSubmenuMetadata<Icon>(SubCommand)?.IconPath;
        public bool IsCheckable => CommandExtractor.GetSubmenuMetadata<Checkable>(SubCommand).IfNotNull(o => o.Value);
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                RaisePropertyChanged(() => IsChecked);
                CommandExtractor.GetSubmenuMetadata<CheckChanged>(SubCommand)?.OnCheckChanged?.Invoke(value);
            }
        }
        public string ToolTip => CommandExtractor.GetSubmenuMetadata<ToolTip>(SubCommand)?.Value;
        
        public MainMenuSubCommandViewModel(IObjectInitializationService initSvc, IMainMenuCommandExtractor commandExtractor, ISubCommand subCommand)
            : base(initSvc)
        {
            CommandExtractor = commandExtractor;
            SubCommand = subCommand;
        }
        
    }
}

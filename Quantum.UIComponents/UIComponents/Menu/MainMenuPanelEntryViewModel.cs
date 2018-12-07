using Quantum.Command;
using Quantum.Metadata;
using Quantum.Services;
using System.Linq;

namespace Quantum.UIComponents
{
    internal class MainMenuPanelEntryViewModel : ViewModelBase, IMainMenuItemViewModel
    {
        private IStaticPanelDefinition PanelDefinition { get; }
        private IMainMenuCommandExtractor CommandExtractor { get; }

        public string Header => CommandExtractor.GetPanelMenuOptionMetadata<Description>(PanelDefinition)?.Value;
        public string ToolTip => CommandExtractor.GetPanelMenuOptionMetadata<ToolTip>(PanelDefinition)?.Value;
        public string Icon => CommandExtractor.GetPanelMenuOptionMetadata<Icon>(PanelDefinition)?.IconPath;
        public string Shortcut => PanelDefinition.OfType<BringIntoViewOnKeyShortcut>().SingleOrDefault()?.GetInputGestureText() ?? string.Empty;

        public IDelegateCommand BringIntoView
        {
            get
            {
                return new DelegateCommand()
                {
                    CanExecuteHandler = () => PanelDefinition.OfType<StaticPanelConfiguration>().Single().CanOpen(),
                    ExecuteHandler = () => EventAggregator.GetEvent<BringStaticPanelIntoViewRequest>().Publish(new BringStaticPanelIntoViewArgs(PanelDefinition.IViewModel))
                };
            }
        }


        public MainMenuPanelEntryViewModel(IObjectInitializationService initSvc, IMainMenuCommandExtractor commandExtractor, IStaticPanelDefinition definition)
            : base(initSvc)
        {
            CommandExtractor = commandExtractor;
            PanelDefinition = definition;
        }

        
        [Handles(typeof(StaticPanelInvalidationEvent))]
        public void OnInvalidation(StaticPanelInvalidationArgs args)
        {
            if(args.Definition == PanelDefinition)
            {
                RaisePropertyChanged(() => Header);
                RaisePropertyChanged(() => ToolTip);
                RaisePropertyChanged(() => Icon);
                BringIntoView.RaiseCanExecuteChanged();
            }
        }
        
    }
}

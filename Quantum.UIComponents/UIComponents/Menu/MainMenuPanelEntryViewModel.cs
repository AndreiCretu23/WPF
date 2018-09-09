using Quantum.Command;
using Quantum.Metadata;
using Quantum.Services;
using System.Linq;

namespace Quantum.UIComponents
{
    internal class MainMenuPanelEntryViewModel : ViewModelBase, IMainMenuItemViewModel
    {
        public IStaticPanelDefinition PanelDefinition { get; set; }
        

        public string Header => GetPanelMenuMetadata<Description>()?.Value;
        public string ToolTip => GetPanelMenuMetadata<ToolTip>()?.Value;
        public string Icon => GetPanelMenuMetadata<Icon>()?.IconPath;
        
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


        public MainMenuPanelEntryViewModel(IObjectInitializationService initSvc, IStaticPanelDefinition definition)
            : base(initSvc)
        {
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

        private TMetadata GetPanelMenuMetadata<TMetadata>()
        {
            return PanelDefinition.OfType<PanelMenuOption>().Single().OfType<TMetadata>().SingleOrDefault();
        }
    }
}

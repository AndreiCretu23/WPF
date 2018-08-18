using Quantum.Metadata;
using Quantum.Services;
using System.Linq;

namespace Quantum.UIComponents
{
    internal class MainMenuPanelEntryViewModel : ViewModelBase, IMainMenuItemViewModel
    {
        public IStaticPanelDefinition PanelDefinition { get; set; }

        private string header;
        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                RaisePropertyChanged(() => Header);
            }
        }

        private string toolTip;
        public string ToolTip
        {
            get { return toolTip; }
            set
            {
                toolTip = value;
                RaisePropertyChanged(() => ToolTip);
            }
        }

        private string icon;
        public string Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                RaisePropertyChanged(() => Icon);
            }
        }

        private bool isOpened;
        public bool IsOpened
        {
            get { return isOpened; }
            set
            {
                isOpened = value;
                RaisePropertyChanged(() => IsOpened);
                EventAggregator.GetEvent<PanelMenuEntryStateChangedEvent>().Publish(new PanelMenuEntryStateChangedArgs(PanelDefinition, value));
            }
        }

        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                RaisePropertyChanged(() => IsEnabled);
            }
        }

        public MainMenuPanelEntryViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        [Handles(typeof(StaticPanelVisibilityChangedEvent))]
        public void OnVisibilityChanged(StaticPanelVisibilityChangedArgs args)
        {
            if (args.Definition.IViewModel == PanelDefinition.IViewModel)
            {
                isOpened = args.Visibility;
                RaisePropertyChanged(() => IsOpened);
                IsEnabled = PanelDefinition.CanChangeVisibility(args.Visibility);
            }
        }

        [Handles(typeof(StaticPanelInvalidationEvent))]
        public void OnInvalidation(StaticPanelInvalidationArgs args)
        {
            if(args.Definition == PanelDefinition)
            {
                var configuration = args.Definition.OfType<StaticPanelConfiguration>().Single();
                var visibility = configuration.IsVisible();
                var isEnabled = args.Definition.CanChangeVisibility(visibility);

                isOpened = visibility;
                RaisePropertyChanged(() => IsOpened);

                IsEnabled = isEnabled;
            }
        }
    }
}

using Quantum.Services;
using Quantum.UIComposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //[Handles(typeof(PanelVisibilityChangedEvent))]
        //public void OnVisibilityChanged(PanelVisibilityChangedArgs args)
        //{
        //    if(args.IViewModel == PanelDefinition.IViewModel && 
        //        IsOpened != args.Visibility)
        //    {
        //        IsOpened = args.Visibility;
        //    }
        //}

    }
}

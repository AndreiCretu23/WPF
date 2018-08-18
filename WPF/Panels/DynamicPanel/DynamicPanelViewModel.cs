using Quantum.Common;
using Quantum.Services;
using Quantum.UIComponents;

namespace WPF.Panels
{
    public class DynamicPanelViewModel : ViewModelBase, IDynamicPanelViewModel, IIdentifiable
    {
        public DynamicPanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        

        public string Guid { get; set; }


        private string displayText;
        public string DisplayText
        {
            get { return displayText; }
            set
            {
                displayText = value;
                RaisePropertyChanged(() => DisplayText);
            }
        }
    }
}

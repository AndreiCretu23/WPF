using Quantum.Common;
using Quantum.Services;
using Quantum.UIComponents;

namespace WPF.Panels
{
    public class DynamicPanelViewModel : ViewModelBase, IDynamicPanelViewModel, IIdentifiable
    {
        [Selection]
        public SelectedNumber SelectedNumber { get; set; }

        public DynamicPanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        

        public string Guid { get; set; }


        private string displayText;
        [InvalidateOn(typeof(SelectedNumber))]
        public string DisplayText
        {
            get { return $"{displayText} {SelectedNumber.Value.ToString()}"; }
            set
            {
                displayText = value;
                RaisePropertyChanged(() => DisplayText);
            }
        }
    }
}

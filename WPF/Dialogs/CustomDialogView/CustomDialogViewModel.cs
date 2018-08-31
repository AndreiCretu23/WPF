using Quantum.Command;
using Quantum.Services;
using Quantum.UIComponents;

namespace WPF.Dialogs
{
    public class CustomDialogViewModel : DialogViewModel, ICustomDialogViewModel
    {
        [Selection]
        public SelectedNumber SelectedNumber { get; set; }


        public CustomDialogViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        

        protected override bool ValidateContent()
        {
            return SelectedNumber.Value < 5;
        }

        public IDelegateCommand IncrementNumberCommand
        {
            get
            {
                return new DelegateCommand()
                {
                    CanExecuteHandler = () => SelectedNumber.Value < 11,
                    ExecuteHandler = () => SelectedNumber.Value++
                };
            }
        }

        public IDelegateCommand DecrementNumberCommand
        {
            get
            {
                return new DelegateCommand()
                {
                    CanExecuteHandler = () => SelectedNumber.Value >= 0,
                    ExecuteHandler = () => SelectedNumber.Value--
                };
            }
        }

        [Handles(typeof(SelectedNumber))]
        public void OnSelectedNumberChanged()
        {
            RaiseContentValidationChanged();
            RaisePropertyChanged(() => IncrementNumberCommand);
            RaisePropertyChanged(() => DecrementNumberCommand);
        }

    }
}

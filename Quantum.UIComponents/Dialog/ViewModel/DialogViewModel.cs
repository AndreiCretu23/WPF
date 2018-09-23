using Quantum.Services;
using Quantum.Command;
using System;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Provides the basic abstract view model of a DialogWindow.
    /// </summary>
    public abstract class DialogViewModel : ViewModelBase, IDialogViewModel
    {
        internal event Action<bool> OnCloseRequest;

        public DialogViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            AbortCommand = new DelegateCommand()
            {
                CanExecuteHandler = () => CanAbort(),
                ExecuteHandler = () =>
                {
                    Abort();
                    OnCloseRequest?.Invoke(false);
                }
            };

            SaveCommand = new DelegateCommand()
            {
                CanExecuteHandler = () => ValidateContent(),
                ExecuteHandler = () =>
                {
                    SaveChanges();
                    OnCloseRequest?.Invoke(true);
                }
            };
        }
        

        /// <summary>
        /// An abort command property used for UIBinding. CanExecute returns CanAbort() and Execute returns Abort().
        /// </summary>
        public IDelegateCommand AbortCommand { get; }

        /// <summary>
        /// A save command used for UI Binding. CanExecute returns ValidateContent() and Execute returns SaveChanges().
        /// </summary>
        public IDelegateCommand SaveCommand { get; }
        


        /// <summary>
        /// Gets called when the user attempts to close/cancel/abort the dialog and returns a value indicating if the current state of the dialog allows the operation.
        /// </summary>
        /// <returns></returns>
        protected internal virtual bool CanAbort() { return true; }

        /// <summary>
        /// Gets called when the user cancels/closes/aborts the dialog, if CanAbort() returns true.
        /// </summary>
        protected internal virtual void Abort() { }
        


        /// <summary>
        /// Gets called then the user attempts to close the dialog with an OK dialog result and returns a value indicating if the current state of the dialog allows the operation.
        /// </summary>
        /// <returns></returns>
        protected internal virtual bool ValidateContent() { return true; }

        /// <summary>
        /// Gets called when the user validates the dialog (clicks ok), if ValidateContent() returns true.
        /// </summary>
        protected internal virtual void SaveChanges() { }



        /// <summary>
        /// Raises the abort command's CanExecuteChanged, notifying the UI that the logical CanAbort condition has changed.
        /// </summary>
        protected void RaiseCanAbortChanged()
        {
            AbortCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Raises the save command's CanExecuteChanged, notifying the UI that the logical ValidateContent condition has changed.
        /// </summary>
        protected void RaiseContentValidationChanged()
        {
            SaveCommand.RaiseCanExecuteChanged();
        }
    }
}

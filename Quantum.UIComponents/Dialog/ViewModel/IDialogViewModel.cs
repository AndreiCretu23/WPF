using Quantum.Command;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Provides a contract for a view model of a dialog window.
    /// </summary>
    public interface IDialogViewModel
    {
        IDelegateCommand AbortCommand { get; }
        IDelegateCommand SaveCommand { get; }
    }
}

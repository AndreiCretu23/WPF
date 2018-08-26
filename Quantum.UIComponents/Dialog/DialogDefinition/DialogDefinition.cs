using System;

namespace Quantum.UIComponents
{
    public class DialogDefinition<IView, TView, IViewModel, TViewModel> : IDialogDefinition
        where IView : IDialogWindow
        where TView : DialogWindow, IView, new()
        where IViewModel : IDialogViewModel
        where TViewModel : DialogViewModel, IViewModel
    {
        public Type View => typeof(TView);
        public Type ViewModel => typeof(TViewModel);
        Type IDialogDefinition.IView => typeof(IView);
        Type IDialogDefinition.IViewModel => typeof(IViewModel);
        public bool SingleViewModelInstance { get; }

        public DialogDefinition(bool singleViewModelInstance = false)
        {
            SingleViewModelInstance = singleViewModelInstance;
        }

    }
}

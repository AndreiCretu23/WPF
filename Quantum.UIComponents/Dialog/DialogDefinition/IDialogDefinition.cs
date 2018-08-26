using System;
using System.Windows;

namespace Quantum.UIComponents
{
    public interface IDialogDefinition
    {
        Type IView { get; }
        Type View { get; }
        Type IViewModel { get; }
        Type ViewModel { get; }
        bool SingleViewModelInstance { get; }
    }
}

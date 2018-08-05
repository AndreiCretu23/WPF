using System;

namespace Quantum.UIComponents
{
    public interface IPanelDefinition
    {
        Type IView { get; }
        Type View { get; }
        Type IViewModel { get; }
        Type ViewModel { get; }
    }
}

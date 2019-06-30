using Quantum.Common;
using Quantum.UIComposition;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents the basic contract for any view model.
    /// </summary>
    public interface IViewModel : IObservableObject, IDestructible
    {
    }
}

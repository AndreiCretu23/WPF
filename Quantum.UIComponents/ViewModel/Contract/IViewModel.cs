using Quantum.Common;
using Quantum.UIComposition;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Defines the basic contract for a ViewModel that has access to the container, services and events.
    /// </summary>
    public interface IViewModel : IObservableObject, IDestructible
    {
    }
}

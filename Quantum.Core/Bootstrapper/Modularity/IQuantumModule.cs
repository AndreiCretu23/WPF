using Unity;

namespace Quantum.Core
{
    public interface IQuantumModule
    {
        void Initialize(IUnityContainer container);
    }
}

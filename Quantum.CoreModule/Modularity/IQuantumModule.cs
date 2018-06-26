using Microsoft.Practices.Unity;

namespace Quantum.CoreModule
{
    public interface IQuantumModule
    {
        void Initialize(IUnityContainer container);
    }
}

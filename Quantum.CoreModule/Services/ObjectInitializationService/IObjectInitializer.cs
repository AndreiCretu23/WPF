using Microsoft.Practices.Unity;

namespace Quantum.Services
{
    public interface IObjectInitializer
    {
        IUnityContainer Container { get; set; }

        void Initialize(object obj);
        void Teardown(object obj);
    }
}

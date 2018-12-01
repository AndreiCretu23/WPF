using Quantum.Services;

namespace Quantum.Common
{
    public interface IInitializableObject
    {
        bool IsInitialized { get; }
        void Initialize(IObjectInitializationService initSvc);
        void TearDown();
    }
}

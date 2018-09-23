using Microsoft.Practices.Unity;

namespace Quantum.CoreModule
{
    /// <summary>
    /// Provides the basic contract for module initialization inside the framework. 
    /// When the application starts, all modules that are registered inside the local / framework bootstrapper will 
    /// have their Initialize method called. The modules of an application are responsible for registering all the 
    /// Services / Selections / Events / Definitions that the application is going to use.
    /// </summary>
    public interface IQuantumModule
    {
        void Initialize(IUnityContainer container);
    }
}

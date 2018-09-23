using Microsoft.Practices.Unity;

namespace Quantum.Services
{
    /// <summary>
    /// The basic structure of an object initializer that can be registered in the ObjectInitializationService.
    /// Once registered in the main initialization service, the initialize method of the initializer will get called 
    /// for each object the services initializes.
    /// </summary>
    public interface IObjectInitializer
    {
        /// <summary>
        /// The ObjectInitializationService's associated container. This property will automatically be set by the service before initialized is called.
        /// </summary>
        IUnityContainer Container { get; set; }

        /// <summary>
        /// Called by the object initialization service to perform the initialization of an object given the object's metadata provided via various attributes.
        /// </summary>
        /// <param name="obj"></param>
        void Initialize(object obj);

        /// <summary>
        /// Called by the object initializaton service to perform a teardown of the object for this initializer. Responsible for reverting the initialization process.
        /// </summary>
        /// <param name="obj"></param>
        void Teardown(object obj);
    }
}

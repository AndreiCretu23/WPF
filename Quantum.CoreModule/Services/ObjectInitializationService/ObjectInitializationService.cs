using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.Services
{
    /// <summary>
    /// The basic initialization service for all objects registered in the container. 
    /// It is responsible of injecting the dependencies from the container in the object it initializes depending 
    /// on the metadata provided via attributes on the properties and methods the type contains.
    /// </summary>
    public interface IObjectInitializationService
    {
        /// <summary>
        /// Initializes and injects the dependencies from the associated container in an object depending on the 
        /// metadata provided via attributes on the properties and methods the type contains.
        /// </summary>
        /// <param name="obj"></param>
        void Initialize(object obj);

        /// <summary>
        /// Registers a new object initializer.
        /// </summary>
        /// <typeparam name="TInitializer"></typeparam>
        void RegisterInitializer<TInitializer>() where TInitializer : IObjectInitializer, new();

        /// <summary>
        /// Unregisters an object initializer.
        /// </summary>
        /// <typeparam name="TInitializer"></typeparam>
        void UnregisterInitializer<TInitializer>() where TInitializer : IObjectInitializer, new();

        /// <summary>
        /// Unregisters all object initializers.
        /// </summary>
        void UnregisterAllInitializers();

        /// <summary>
        /// Calls the Teardown method of the specified object initializer for the given object.
        /// </summary>
        /// <typeparam name="TInitializer"></typeparam>
        /// <param name="obj"></param>
        void Teardown<TInitializer>(object obj) where TInitializer : IObjectInitializer, new();

        /// <summary>
        /// Calls the Teardown method of all registered initializers for the given object.
        /// </summary>
        /// <param name="obj"></param>
        void TeardownAll(object obj);
    }

    internal class ObjectInitializationService : IObjectInitializationService
    {
        private IUnityContainer Container { get; set; }
        private List<IObjectInitializer> RegisteredInitializers { get; set; }

        public ObjectInitializationService(IUnityContainer container)
        {
            this.Container = container;
            RegisteredInitializers = new List<IObjectInitializer>();
        }

        public void Initialize(object obj)
        {
            foreach (var initializer in RegisteredInitializers)
            {
                initializer.Initialize(obj);
            }
        }

        public void RegisterInitializer<TInitializer>()
            where TInitializer : IObjectInitializer, new()
        {
            if (this.RegisteredInitializers.OfType<TInitializer>().Any()) return;

            RegisteredInitializers.Add(new TInitializer()
            {
                Container = Container
            });
        }

        public void UnregisterInitializer<TInitializer>()
            where TInitializer : IObjectInitializer, new()
        {
            var unregisterRequest = this.RegisteredInitializers.OfType<TInitializer>().ToList();

            foreach (var initializer in unregisterRequest)
            {
                this.RegisteredInitializers.Remove(initializer);
            }
        }

        public void UnregisterAllInitializers()
        {
            RegisteredInitializers.Clear();
        }

        public void Teardown<TInitializer>(object obj)
            where TInitializer : IObjectInitializer, new()
        {
            RegisteredInitializers.OfType<TInitializer>().Single().Teardown(obj);
        }

        public void TeardownAll(object obj)
        {
            foreach (var initializer in RegisteredInitializers)
            {
                initializer.Teardown(obj);
            }
        }
    }
}

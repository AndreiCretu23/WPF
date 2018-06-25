using System.Collections.Generic;
using System.Linq;
using Unity;

namespace Quantum.Services
{
    public interface IObjectInitializationService
    {
        void Initialize(object obj);

        void RegisterInitializer<TInitializer>() where TInitializer : IObjectInitializer, new();
        void UnregisterInitializer<TInitializer>() where TInitializer : IObjectInitializer, new();
        void UnregisterAllInitializers();

        void Teardown<TInitializer>(object obj) where TInitializer : IObjectInitializer, new();
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
            var unregisterRequest = this.RegisteredInitializers.OfType<TInitializer>();

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

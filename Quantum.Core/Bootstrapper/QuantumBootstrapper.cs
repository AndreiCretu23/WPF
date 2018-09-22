using Microsoft.Practices.Unity;
using Quantum.CoreModule;
using Quantum.Services;
using Quantum.UIComponents;
using System.Collections.Generic;

namespace Quantum.Core
{
    public abstract class QuantumBootstrapper 
    {
        private IUnityContainer CreateContainer()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<IUnityContainer>(container);
            return container;
        }
        
        private IFrameworkConfig RegisterFrameworkConfig(IUnityContainer container)
        {
            container.RegisterService<IFrameworkConfig, FrameworkConfig>();
            return container.Resolve<IFrameworkConfig>();
        }

        protected virtual void OverrideConfigMetadata(IUnityContainer container, IFrameworkConfig config) { }

        private IEnumerable<IQuantumModule> GetFrameworkModules()
        {
            yield return new QuantumCoreModule();
            yield return new QuantumUIModule();
        }
        
        protected abstract IEnumerable<IQuantumModule> GetApplicationModules();

        private void CreateShell(IUnityContainer container)
        {
            container.Resolve<IUICoreService>().CreateUI();
        }

        public void Run()
        {
            var container = CreateContainer();
            OverrideConfigMetadata(container, RegisterFrameworkConfig(container));

            foreach(var frameworkModule in GetFrameworkModules())
            {
                frameworkModule.Initialize(container);
            }
            foreach(var applicationModule in GetApplicationModules())
            {
                applicationModule.Initialize(container);
            }

            CreateShell(container);
        }
    }
}

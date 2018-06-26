using Microsoft.Practices.Unity;
using Quantum.CoreModule;
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

        public abstract IEnumerable<IQuantumModule> GetApplicationModules();

        private IEnumerable<IQuantumModule> GetFrameworkModules()
        {
            yield return new QuantumCoreModule();
            yield return new QuantumUIModule();
        }
        
        private void CreateShell(IUnityContainer container)
        {
            container.Resolve<IUICoreService>().CreateUI();
        }

        public void Run()
        {
            var container = CreateContainer();

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

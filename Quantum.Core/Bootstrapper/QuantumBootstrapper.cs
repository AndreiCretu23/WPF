using Quantum.CoreModule;
using System.Collections.Generic;
using Unity;

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
        }
        
        private void CreateUI()
        {

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

            CreateUI();
        }
    }
}

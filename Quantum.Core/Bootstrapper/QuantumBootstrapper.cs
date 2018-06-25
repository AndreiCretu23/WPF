using Quantum.Core.Services;
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

        private IEnumerable<IQuantumModule> GetFrameworkModules()
        {
            yield return new BaseModule();
        }

        public abstract IEnumerable<IQuantumModule> GetApplicationModules();

        public void Run()
        {

        }
    }
}

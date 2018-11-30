using Microsoft.Practices.Unity;
using Quantum.CoreModule;
using Quantum.Services;
using Quantum.UIComponents;
using System.Collections.Generic;

namespace Quantum.Core
{
    /// <summary>
    /// This class represents the main logic of the initialization of the framework and the application that is to be run.
    /// It is responsible for creating the IOC container of the application, registering all the modules and services 
    /// (both framework modules and application modules) and creating the MainWindow.
    /// In order to run an application using this framework, create a class that extends QuantumBootstrapper and configure it.
    /// Then, from the StartUp method of the application(App.xaml.cs, override OnStartUp), instantiate the local app bootstrapper and call the run method.
    /// </summary>
    public abstract class QuantumBootstrapper 
    {
        /// <summary>
        /// Creates the IOC container of the application. All the services, viewModels, events and selections will be registered in this container.
        /// </summary>
        /// <returns></returns>
        private IUnityContainer CreateContainer()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<IUnityContainer>(container);
            return container;
        }
        
        /// <summary>
        /// Returns the main framework modules. <para></para>
        /// CoreModule -> Contains services used by all the other modules. Provides basic utilities for the IOC container, basic services, events, selections, etc 
        /// and acts like an interface for the main components of the framework.<para></para>
        /// UIModule -> Contains services and utilities responsible for the UI of the application : MainView : Menu, Toolbar, Paneling, Dialogs, etc.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IQuantumModule> GetFrameworkModules()
        {
            yield return new QuantumCoreModule();
            yield return new QuantumUIModule();
        }
        
        /// <summary>
        /// Override this method in the local application bootstrapper in order to provide the framework the modules containing all the services/events/selections/definitions.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<IQuantumModule> GetApplicationModules();

        /// <summary>
        /// Creates the MainWindow of the application.
        /// </summary>
        /// <param name="container"></param>
        private void CreateShell(IUnityContainer container)
        {
            container.Resolve<IUICoreService>().CreateUI();
        }

        /// <summary>
        /// Runs the application : It creates the container, registers the framework modules, 
        /// then the application modules, and finally creates the MainWindow of the application.
        /// </summary>
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

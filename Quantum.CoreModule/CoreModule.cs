using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using Quantum.Events;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Quantum.CoreModule
{
    public class QuantumCoreModule : IQuantumModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterService<IObjectInitializationService, ObjectInitializationService>();
            container.Resolve<IObjectInitializationService>().RegisterInitializer<ServiceInitializer>();
            container.Resolve<IObjectInitializationService>().RegisterInitializer<SelectionInitializer>();
            container.Resolve<IObjectInitializationService>().RegisterInitializer<SubscriberInitializer>();

            container.RegisterService<IEventAggregator, UnityEventAggregator>();

            container.RegisterService<IWPFEventManagerService, WPFEventManagerService>();
            container.Resolve<IWPFEventManagerService>().HookWpfEvents();

            
            container.RegisterService<IConfigManagerService, ConfigManagerService>();
            
        }

    }
    

}

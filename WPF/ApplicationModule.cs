﻿using Quantum.CoreModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quantum.Services;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Events;

namespace WPF
{
    public class ApplicationModule : IQuantumModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterService<IDummyService, DummyService>();
            
            var selection = container.Resolve<DummySelection>();
            selection.Value = 15;


            container.Resolve<IDummyService>().TestMethod();
        }
        
    }
}
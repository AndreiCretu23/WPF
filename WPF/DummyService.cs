﻿using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Services;
using Quantum.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace WPF
{
    public interface IDummyService
    {
        void TestMethod();
    }

    public class DummyService : ServiceBase, IDummyService
    {
        public DummyService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        [Handles(typeof(ShutdownEvent))]
        public void OnAppExit()
        {
            MessageBox.Show("Application Exiting");
        }

        [Handles(typeof(UnhandledExceptionEvent))]
        public void OnUnhandledException()
        {
            MessageBox.Show("App Crashed!");
        }

        [Handles(typeof(SelectedNumber), ThreadOption.UIThread)]
        public void TestMethod()
        {
            //Thread.Sleep(10000);
        }
    }
    
    public class SelectedNumber : SingleSelection<int>
    {
    }
}

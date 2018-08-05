using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Services;
using Quantum.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF
{
    public interface IDummyService
    {
        void TestMethod();
    }

    public class DummyService : QuantumServiceBase, IDummyService
    {
        public DummyService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        [Handles(typeof(ApplicationExitEvent))]
        public void OnAppExit()
        {
            MessageBox.Show("Application Exiting");
        }

        [Handles(typeof(UnhandledExceptionEvent))]
        public void OnUnhandledException()
        {
            MessageBox.Show("App Crashed!");
        }


        public void TestMethod()
        {
            // Testing Code
        }
   
    }
    
    public class SelectedNumber : SingleSelection<int>
    {
    }
}

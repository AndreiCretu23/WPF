using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Services;
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

        public void TestMethod()
        {
            // Do Nothing.
        }

    }


    public class TestingEvent : CompositePresentationEvent<int> { }

    public class SelectedNumber : SingleSelection<int>
    {
    }

}

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

        
        [Selection]
        public DummySelection DummySelection { get; set; }

        public void TestMethod()
        {
            MessageBox.Show(DummySelection.Value.ToString());
        }
    }


    public class DummyEvent : CompositePresentationEvent<int> { }

    public class DummySelection : SingleSelection<int>
    {
    }

}

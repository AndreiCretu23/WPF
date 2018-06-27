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
    public interface IDummyService { }

    public class DummyService : QuantumServiceBase, IDummyService
    {
        public DummyService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            //EventAggregator.GetEvent<DummySelection>().Subscribe(o => Handle(), ThreadOption.PublisherThread, true);
            //EventAggregator.GetEvent<DummySelection>().Subscribe(o => HandleArgs((DummySelection)o), ThreadOption.PublisherThread, true);
        }

        [Handles(typeof(DummySelection))]
        public void Handle()
        {
            MessageBox.Show("No Args");
        }

        [Handles(typeof(DummySelection))]
        public void HandleArgs(DummySelection args)
        {
            MessageBox.Show($"Args = {args.Value}");
        }
    }


    public class DummyEvent : CompositePresentationEvent<int> { }

    public class DummySelection : SingleSelection<int>
    {
        public DummySelection(IObjectInitializationService initSvc)
            : base(initSvc)
        { 
        }
    }

}

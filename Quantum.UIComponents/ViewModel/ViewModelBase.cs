using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Unity;
using Quantum.Common;
using Quantum.Services;
using Quantum.UIComposition;
using System.Linq;

namespace Quantum.UIComponents
{
    public abstract class ViewModelBase : ObservableObject, IDestructible
    {
        [Service]
        public IUnityContainer Container { get; set; }

        [Service]
        public IEventAggregator EventAggregator { get; set; }

        [Service]
        public IObjectInitializationService InitializationService { get; set; }
        
        public ViewModelBase(IObjectInitializationService initSvc)
        {
            initSvc.Initialize(this);
        }
        
        /// <summary>
        /// Tears down all services/events/selections initialized by the IObjectInitializationService
        /// </summary>
        public void TearDown()
        {
            InitializationService.TeardownAll(this);
        }
    }
}

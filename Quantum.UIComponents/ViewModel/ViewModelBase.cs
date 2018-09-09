using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using Quantum.Services;
using Quantum.UIComposition;

namespace Quantum.UIComponents
{
    public abstract class ViewModelBase : ObservableObject
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
        protected void TearDown()
        {
            InitializationService.TeardownAll(this);
        }
    }
}

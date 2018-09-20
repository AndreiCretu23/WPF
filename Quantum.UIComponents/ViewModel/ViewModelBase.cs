using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Unity;
using Quantum.Services;
using Quantum.UIComposition;
using System.Linq;

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
            ResolveInvalidationRequests();
        }

        /// <summary>
        /// Tears down all services/events/selections initialized by the IObjectInitializationService
        /// </summary>
        protected void TearDown()
        {
            InitializationService.TeardownAll(this);
        }

        private void ResolveInvalidationRequests()
        {
            var properties = GetType().GetProperties();
            foreach(var prop in properties)
            {
                var invalidationAttributes = prop.GetCustomAttributes(true).OfType<InvalidateOnAttribute>();
                foreach(var attribute in invalidationAttributes)
                {
                    EventAggregator.Subscribe(attribute.EventType, () => RaisePropertyChanged(prop.Name), ThreadOption.UIThread);
                }
            }
        }
        
    }
}

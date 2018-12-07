using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using Quantum.Common;
using Quantum.Services;
using Quantum.UIComposition;

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
        /// Tears down all injected services/selection and subscribed event handlers initialized by the IObjectInitializationService.
        /// Gets called by various components of the framework when the UIElement associated with this ViewModel is disposed/invalidated.
        /// </summary>
        public virtual void TearDown()
        {
            InitializationService.TeardownAll(this);
        }
    }
}

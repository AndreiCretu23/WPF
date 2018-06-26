using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;

namespace Quantum.Services
{
    public interface ISelection
    {
        void ForcePublish();
        SubscriptionToken Subscribe(IDelegateReference actionReference, ThreadOption threadOption, bool keepSubscriberReferenceAlive);
        void Unsubscribe(SubscriptionToken token);
    }
}

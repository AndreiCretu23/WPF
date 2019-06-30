using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.UIComponents
{
    internal class ReferencePathExpansionRetainer : ITreeExpansionRetainer
    {
        public ITreeViewModel Owner { get; }
        private IList<ReferenceExpansionRetainer> ItemStates { get; } = new List<ReferenceExpansionRetainer>();
        private int CleanUpRequests { get; set; }

        public ReferencePathExpansionRetainer(ITreeViewModel owner)
        {
            Owner = owner;
        }

        public void LoadExpansionState(ITreeViewModelItem treeViewModelItem)
        {
            var isExpanded = ItemStates.SingleOrDefault(o => o.Item.IsAlive && o.Item.Target.Equals(treeViewModelItem.Value))?.
                             GetExpansionState(treeViewModelItem.GetAncestors().Select(o => o.Value));
            if (isExpanded != null)
            {
                treeViewModelItem.IsExpanded = (bool)isExpanded;
            }
        }


        public void SaveExpansionState(ITreeViewModelItem treeViewModelItem)
        {
            var currentStates = ItemStates.SingleOrDefault(o => o.Item.IsAlive && o.Item.Target.Equals(treeViewModelItem.Value));
            if (currentStates != null)
            {
                currentStates.SetExpansionState(treeViewModelItem.GetAncestors().Select(o => o.Value), treeViewModelItem.IsExpanded);
            }
            else
            {
                var stateRetainer = new ReferenceExpansionRetainer(treeViewModelItem.Value);
                stateRetainer.SetExpansionState(treeViewModelItem.GetAncestors().Select(o => o.Value), treeViewModelItem.IsExpanded);
                ItemStates.Add(stateRetainer);
            }
        }

        public void CleanUp()
        {
            CleanUpRequests++;

            if(CleanUpRequests % 3 == 0)
            {
                ItemStates.RemoveWhere(o => !o.Item.IsAlive);
                foreach (var stateCollection in ItemStates)
                {
                    stateCollection.CleanUp();
                }
            }
            
            if(CleanUpRequests % 6 == 0)
            {
                var allValues = Owner.Items.Select(o => o.Value).Distinct().ToList();
                ItemStates.RemoveWhere(o => !allValues.Contains(o.Item.Target));
                
                foreach(var stateCollection in ItemStates) {
                    stateCollection.CleanContexts(Owner.Items.Where(o => stateCollection.Item.Target.Equals(o.Value))
                                                             .Select(o => o.GetAncestors()));
                }

            }

            if(CleanUpRequests > 5)
            {
                CleanUpRequests = 0;
            }
        }
    }



    internal class ReferenceExpansionRetainer
    {
        public WeakReference Item { get; }
        private IList<ReferenceExpansionContext> Contexts { get; } = new List<ReferenceExpansionContext>();

        public ReferenceExpansionRetainer(object item)
        {
            Item = new WeakReference(item);
        }

        public bool? GetExpansionState(IEnumerable<object> context)
        {
            return Contexts.SingleOrDefault(o => o.ContextEquals(context))?.IsExpanded;
        }

        public void SetExpansionState(IEnumerable<object> context, bool isExpanded)
        {
            var currentContext = Contexts.SingleOrDefault(o => o.ContextEquals(context));
            if (currentContext != null)
            {
                currentContext.IsExpanded = isExpanded;
            }

            else
            {
                Contexts.Add(new ReferenceExpansionContext()
                {
                    Context = context.Select(o => new WeakReference(o)).ToList(),
                    IsExpanded = isExpanded
                });
            }
        }

        public void CleanUp()
        {
            Contexts.RemoveWhere(o => !o.IsContextValid());
        }

        public void CleanContexts(IEnumerable<IEnumerable<object>> currentContexts)
        {
            Contexts.RemoveWhere(o => !currentContexts.Any(c => o.ContextEquals(c)));
        }
    }

    internal class ReferenceExpansionContext
    {
        public IEnumerable<WeakReference> Context { get; set; }
        public bool IsExpanded { get; set; }

        public bool ContextEquals(IEnumerable<object> otherContext)
        {
            return IsContextValid() && Context.Select(o => o.Target).SequenceEqual(otherContext);
        }

        public bool IsContextValid()
        {
            return Context.All(o => o.IsAlive);
        }
    }
}

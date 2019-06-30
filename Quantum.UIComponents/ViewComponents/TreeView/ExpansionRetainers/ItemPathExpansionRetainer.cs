using System.Collections.Generic;
using System.Linq;

namespace Quantum.UIComponents
{
    internal class ItemPathExpansionRetainer : ITreeExpansionRetainer
    {
        public ITreeViewModel Owner { get; }
        private Dictionary<string, bool> ExpansionStates { get; } = new Dictionary<string, bool>();
        private int CleanUpRequests { get; set; }

        public ItemPathExpansionRetainer(ITreeViewModel owner)
        {
            Owner = owner;
        }
        
        public void LoadExpansionState(ITreeViewModelItem item)
        {
            var itemPath = item.CreatePath();
            if(ExpansionStates.ContainsKey(itemPath))
            {
                item.IsExpanded = ExpansionStates[itemPath];
            }
        }

        public void SaveExpansionState(ITreeViewModelItem item)
        {
            var itemPath = item.CreatePath();
            if(ExpansionStates.ContainsKey(itemPath))
            {
                ExpansionStates[itemPath] = item.IsExpanded;
            }
            else
            {
                ExpansionStates.Add(itemPath, item.IsExpanded);
            }
        }
        
        public void CleanUp()
        {
            CleanUpRequests++;
            if(CleanUpRequests < 10) {
                return;
            }
            
            var allItemPaths = Owner.Items.Select(o => o.CreatePath()).ToList();
            foreach(var state in ExpansionStates.ToList())
            {
                if(!allItemPaths.Contains(state.Key)) {
                    ExpansionStates.Remove(state.Key);
                }
            }

            CleanUpRequests = 0;
        }
    }
}

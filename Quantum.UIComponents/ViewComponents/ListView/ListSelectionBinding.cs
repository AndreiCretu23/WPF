using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Common;
using Quantum.Services;
using Quantum.UIComponents;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace Quantum.UIComponents
{

    internal interface IListSelectionBinding : IDestructible
    {
        ISelection BoundSelection { get; }

        void Initialize(IListViewModel viewModel);

        bool SyncItems { get; set; }

        void OnItemSelectionStatusChanged(IListViewModelItem viewModelItem);

        bool IsContainedInSelection(IListViewModelItem value);

        void OnItemsChanging(IEnumerable<IListViewModelItem> newItems);
    }

    internal class SingleListSelectionBinding<T> : IListSelectionBinding
    {
        public IListViewModel ListViewModel { get; private set; }
        public SingleSelection<T> Selection { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool SyncItems { get; set; }
        private Timer Timer;
        private Subscription SelectionSubscription;
        private IListViewModelItem LastSelectedItem;

        private Scope SelectionChangedScope;
        private Scope ExternalChangedScope;

        ISelection IListSelectionBinding.BoundSelection => Selection;

        public SingleListSelectionBinding(SingleSelection<T> selection)
        {
            Selection = selection;
        }

        public void Initialize(IListViewModel viewModel)
        {
            if (IsInitialized) {
                throw new Exception("Error : Attempting to initialize an already initialized list selection binding.");
            }

            ListViewModel = viewModel;
            Timer = new Timer()
            {
                AutoReset = false,
                Interval = 100,
            };
            Timer.Elapsed += OnTimerElapsed;
            SelectionChangedScope = new Scope();
            ExternalChangedScope = new Scope();
            SelectionSubscription = new Subscription()
            {
                Event = Selection,
                Token = Selection.Subscribe(o => OnSelectionChanged(), ThreadOption.PublisherThread)
            };

            foreach(var item in ListViewModel.Items)
            {
                item.IsSelected = item.Value?.Equals(Selection.Value) ?? false;
            }


            IsInitialized = true;
        }

        public void TearDown()
        {
            if(!IsInitialized) {
                throw new Exception("Error : Attempting to initialize an already initialized list selection binding.");
            }

            ListViewModel = null;
            Selection = null;

            if(Timer.Enabled) {
                Timer.Stop();
            }

            Timer.Elapsed -= OnTimerElapsed;
            Timer.Dispose();
            Timer = null;

            SelectionChangedScope = null;
            ExternalChangedScope = null;

            SelectionSubscription.Break();
            SelectionSubscription = null;
            
            IsInitialized = false;
        }

        public bool IsContainedInSelection(IListViewModelItem item)
        {
            return item.Value?.Equals(Selection.Value) ?? false;
        }

        

        private void OnSelectionChanged()
        {
            if(ExternalChangedScope.IsInScope) {
                return;
            }

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                using (SelectionChangedScope.BeginScope())
                {
                    foreach(var item in ListViewModel.Items)
                    {
                        item.IsSelected = item.Value?.Equals(Selection.Value) ?? false;
                    }
                }
            }));
        }

        public void OnItemSelectionStatusChanged(IListViewModelItem viewModelItem)
        {
            if(SelectionChangedScope.IsInScope) {
                return;
            }

            if (Timer.Enabled) {
                Timer.Stop();
            }
        
            if(LastSelectedItem == viewModelItem && !viewModelItem.IsSelected)
            {
                LastSelectedItem = null;
            }
            else if(viewModelItem.IsSelected)
            {
                LastSelectedItem = viewModelItem;
            }
        
            Timer.Start();
            
        }
        
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if(LastSelectedItem == null)
            {
                return;
            }

            using (ExternalChangedScope.BeginScope())
            {
                Selection.Value = (T)LastSelectedItem?.Value;
            }

            LastSelectedItem = null;
            Timer.Stop();
        }

        public void OnItemsChanging(IEnumerable<IListViewModelItem> newItems)
        {
            using (ExternalChangedScope.BeginScope())
            {
                if (!newItems.Select(o => o.Value).Contains(Selection.Value))
                {
                    Selection.Value = default;
                }
            }
        }
    }
   

    internal class MultipleListSelectionBinding<T> : IListSelectionBinding
    {
        public IListViewModel ListViewModel { get; private set; }
        public bool SyncItems { get; set; }
        public MultipleSelection<T> Selection { get; private set; }
        public bool IsInitialized { get; private set; }
        private Timer Timer;
        private Subscription SelectionSubscription;
        private IList<IListViewModelItem> SelectionChangedItems = new List<IListViewModelItem>();

        private Scope SelectionChangedScope;
        private Scope ExternalChangedScope;

        ISelection IListSelectionBinding.BoundSelection => Selection;

        public MultipleListSelectionBinding(MultipleSelection<T> selection)
        {
            Selection = selection;
        }

        public void Initialize(IListViewModel viewModel)
        {
            if (IsInitialized)
            {
                throw new Exception("Error : Attempting to initialize an already initialized list selection binding.");
            }

            ListViewModel = viewModel;
            Timer = new Timer()
            {
                AutoReset = false,
                Interval = 100,
            };
            Timer.Elapsed += OnTimerElapsed;
            SelectionChangedScope = new Scope();
            ExternalChangedScope = new Scope();
            SelectionSubscription = new Subscription()
            {
                Event = Selection,
                Token = Selection.Subscribe(o => OnSelectionChanged(), ThreadOption.PublisherThread)
            };

            foreach(var item in ListViewModel.Items)
            {
                item.IsSelected = Selection.Value.Contains((T)item.Value);
            }

            IsInitialized = true;
        }

        public void TearDown()
        {
            if (!IsInitialized)
            {
                throw new Exception("Error : Attempting to initialize an already initialized list selection binding.");
            }

            ListViewModel = null;
            Selection = null;

            if (Timer.Enabled)
            {
                Timer.Stop();
            }

            Timer.Elapsed -= OnTimerElapsed;
            Timer.Dispose();
            Timer = null;

            SelectionChangedScope = null;
            ExternalChangedScope = null;

            SelectionSubscription.Break();
            SelectionSubscription = null;

            IsInitialized = false;
        }


        public bool IsContainedInSelection(IListViewModelItem item)
        {
            return Selection.Value.Contains((T)item.Value);
        }



        private void OnSelectionChanged()
        {
            if (ExternalChangedScope.IsInScope)
            {
                return;
            }

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                using (SelectionChangedScope.BeginScope())
                {
                    foreach (var item in ListViewModel.Items)
                    {
                        item.IsSelected = IsContainedInSelection(item);
                    }
                }
            }));
        }

        public void OnItemSelectionStatusChanged(IListViewModelItem viewModelItem)
        {
            if (SelectionChangedScope.IsInScope)
            {
                return;
            }

            if (Timer.Enabled)
            {
                Timer.Stop();
            }

            if(!SelectionChangedItems.Contains(viewModelItem))
            {
                SelectionChangedItems.Add(viewModelItem);
            }
            
            Timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            using (ExternalChangedScope.BeginScope())
            {
                using (Selection.BeginBlockingNotifications())
                {
                    foreach(var item in SelectionChangedItems)
                    {
                        if(item.IsSelected && !Selection.Value.Contains((T)item.Value))
                        {
                            Selection.Add((T)item.Value);
                        }

                        else if(!item.IsSelected && Selection.Value.Contains((T)item.Value))
                        {
                            Selection.Remove((T)item.Value);
                        }
                    }

                    if (SyncItems)
                    {
                        foreach (var item in ListViewModel.Items)
                        {
                            item.IsSelected = Selection.Value.Contains((T)item.Value);
                        }
                    }
                }
            }

            SelectionChangedItems.Clear();
            Timer.Stop();
        }

        public void OnItemsChanging(IEnumerable<IListViewModelItem> newItems)
        {
            using (ExternalChangedScope.BeginScope())
            {
                using (Selection.BeginBlockingNotifications())
                {
                    foreach (var item in newItems)
                    {
                        if (!Selection.Value.Contains((T)item.Value))
                        {
                            Selection.Remove((T)item.Value);
                        }
                    }
                }
            }
        }
    }
    
}

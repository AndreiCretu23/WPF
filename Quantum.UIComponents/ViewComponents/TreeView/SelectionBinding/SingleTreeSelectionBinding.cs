using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace Quantum.UIComponents
{
    internal class SingleTreeSelectionBinding<T> : ITreeSelectionBinding
    {
        public ITreeViewModel Owner { get; private set; }
        public SingleSelection<T> Selection { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool SyncItems { get; set; }
        private Timer Timer;
        private Subscription SelectionSubscription;
        private ITreeViewModelItem LastSelectedItem;

        private Scope SelectionChangedScope;

        ISelection ITreeSelectionBinding.BoundSelection => Selection;


        public ITreeViewModelItem SelectedItem { get; private set; }
        public IEnumerable<ITreeViewModelItem> SelectedItems => SelectedItem == null ? new List<ITreeViewModelItem>() :
                                                                                       new List<ITreeViewModelItem>() { SelectedItem };



        public SingleTreeSelectionBinding(SingleSelection<T> selection)
        {
            Selection = selection;
        }

        public void Initialize(ITreeViewModel viewModel)
        {
            if (IsInitialized)
            {
                throw new Exception("Error : Attempting to initialize an already initialized list selection binding.");
            }

            Owner = viewModel;
            Timer = new Timer()
            {
                AutoReset = false,
                Interval = 100,
            };
            Timer.Elapsed += OnTimerElapsed;
            SelectionChangedScope = new Scope();
            SelectionSubscription = new Subscription()
            {
                Event = Selection,
                Token = Selection.Subscribe(o => OnSelectionChanged(), ThreadOption.PublisherThread)
            };

            foreach (var item in Owner.Items)
            {
                item.IsSelected = item.Value?.Equals(Selection.Value) ?? false;
            }


            IsInitialized = true;
        }

        public void TearDown()
        {
            if (!IsInitialized)
            {
                throw new Exception("Error : Attempting to initialize an already initialized list selection binding.");
            }

            Owner = null;
            Selection = null;

            if (Timer.Enabled)
            {
                Timer.Stop();
            }

            Timer.Elapsed -= OnTimerElapsed;
            Timer.Dispose();
            Timer = null;

            SelectionChangedScope = null;

            SelectionSubscription.Break();
            SelectionSubscription = null;
            SelectedItem = null;

            IsInitialized = false;
        }

        public bool IsContainedInSelection(ITreeViewModelItem item)
        {
            return item.Value?.Equals(Selection.Value) ?? false;
        }





        private void OnSelectionChanged()
        {
            if (SelectionChangedScope.IsInScope)
            {
                return;
            }

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                using (SelectionChangedScope.BeginScope())
                {
                    foreach (var item in Owner.Items)
                    {
                        if(IsContainedInSelection(item)) {
                            if(SelectedItem != null) {
                                SelectedItem.IsSelected = false;
                            }

                            item.IsSelected = true;
                            SelectedItem = item;
                            break;
                        }
                    }
                }
            }));
        }

        public void OnItemSelectionStatusChanged(ITreeViewModelItem viewModelItem)
        {
            if (SelectionChangedScope.IsInScope) {
                return;
            }

            if (Timer.Enabled) {
                Timer.Stop();
            }

            if (LastSelectedItem == viewModelItem && !viewModelItem.IsSelected)
            {
                LastSelectedItem = null;
            }
            else if (viewModelItem.IsSelected)
            {
                LastSelectedItem = viewModelItem;
            }

            Timer.Start();

        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                if(SelectedItem != null) {
                    SelectedItem.IsSelected = false;
                }
                
                SelectedItem = LastSelectedItem;
                using (SelectionChangedScope.BeginScope()) {
                    Selection.Value = (T)LastSelectedItem?.Value;
                }

                LastSelectedItem = null;
                Timer.Stop();
            }));
        }



        public void OnItemCreated(ITreeViewModelItem viewModelItem)
        {
            if(SelectedItem == null && IsContainedInSelection(viewModelItem)) {
                using (SelectionChangedScope.BeginScope()) {
                    viewModelItem.IsSelected = true;
                    SelectedItem = viewModelItem;
                }
            }
        }

        public void OnItemDisposed(ITreeViewModelItem viewModelItem)
        {
            if(SelectedItem == viewModelItem && viewModelItem.IsSelected) {
                using (SelectionChangedScope.BeginScope()) {
                    viewModelItem.IsSelected = false;
                    SelectedItem = null;
                }
            }
        }
    }
}

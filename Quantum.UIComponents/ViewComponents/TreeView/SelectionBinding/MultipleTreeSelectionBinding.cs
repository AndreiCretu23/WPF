﻿using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace Quantum.UIComponents
{
    internal class MultipleTreeSelectionBinding<T> : ITreeSelectionBinding
    {
        public ITreeViewModel Owner { get; private set; }
        public bool SyncItems { get; set; }
        public MultipleSelection<T> Selection { get; private set; }
        public bool IsInitialized { get; private set; }
        private Timer Timer;
        private Subscription SelectionSubscription;
        private IList<ITreeViewModelItem> SelectionChangedItems = new List<ITreeViewModelItem>();

        private Scope SelectionChangedScope;

        ISelection ITreeSelectionBinding.BoundSelection => Selection;

        public ITreeViewModelItem SelectedItem => SelectedItems.IsSingleElement() ? SelectedItems.Single() : null;

        private readonly IList<ITreeViewModelItem> selectedItems = new List<ITreeViewModelItem>();
        public IEnumerable<ITreeViewModelItem> SelectedItems => selectedItems;

        public MultipleTreeSelectionBinding(MultipleSelection<T> selection)
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

            IsInitialized = false;
        }
        
        public bool IsContainedInSelection(ITreeViewModelItem item)
        {
            return Selection.Value.Contains((T)item.Value);
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
                    selectedItems.Clear();
                    foreach (var item in Owner.Items) {
                        item.IsSelected = IsContainedInSelection(item);
                        if(item.IsSelected) {
                            selectedItems.Add(item);
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

            if (!SelectionChangedItems.Contains(viewModelItem)) {
                SelectionChangedItems.Add(viewModelItem);
            }

            Timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                using (SelectionChangedScope.BeginScope())
                {
                    using (Selection.BeginBlockingNotifications())
                    {
                        foreach (var item in SelectionChangedItems)
                        {
                            if(item.IsSelected && !SelectedItems.Contains(item))
                            {
                                selectedItems.Add(item);
                            }

                            else if(!item.IsSelected && SelectedItems.Contains(item))
                            {
                                selectedItems.Remove(item);
                            }

                            if (item.IsSelected && !IsContainedInSelection(item))
                            {
                                Selection.Add((T)item.Value);
                            }

                            else if (!item.IsSelected && IsContainedInSelection(item))
                            {
                                Selection.Remove((T)item.Value);
                            }
                        }

                        if (SyncItems)
                        {
                            foreach (var item in Owner.Items.Where(o => !o.IsSelected && IsContainedInSelection(o)))
                            {
                                item.IsSelected = true;
                                selectedItems.Add(item);
                            }
                        }
                    }
                }

                SelectionChangedItems.Clear();
                Timer.Stop();

            }));
        }



        public void OnItemCreated(ITreeViewModelItem viewModelItem)
        {
            if(IsContainedInSelection(viewModelItem)) {
                using (SelectionChangedScope.BeginScope()) {
                    viewModelItem.IsSelected = true;
                    selectedItems.Add(viewModelItem);
                }
            }
        }

        public void OnItemDisposed(ITreeViewModelItem viewModelItem)
        {
            if(SelectedItems.Contains(viewModelItem)) {
                using (SelectionChangedScope.BeginScope()) {
                    viewModelItem.IsSelected = false;
                    selectedItems.Remove(viewModelItem);
                }
            }
        }
    }
}

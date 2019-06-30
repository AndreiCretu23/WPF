using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents the basic data context model for a tree view.
    /// </summary>
    public abstract class TreeViewModel : ViewModelBase, ITreeViewModel
    {
        [Service]
        public IIconManagerService IconManager { get; set; }

        private bool allowMultipleSelection = true;
        /// <summary>
        /// Gets or sets a value indicating if multiple selection is allowed in the context of this tree view model.
        /// NOTE : Setting this property breaks the current selection binding (if any).
        /// </summary>
        public bool AllowMultipleSelection
        {
            get { return allowMultipleSelection; }
            protected set
            {
                if (allowMultipleSelection != value)
                {
                    allowMultipleSelection = value;
                    if (HasBoundSelection)
                    {
                        BreakSelectionBinding();
                    }

                    RaisePropertyChanged(nameof(AllowMultipleSelection));
                }
            }
        }


        /// <summary>
        /// Represents an internal collection storing all currently active items.
        /// </summary>
        private readonly IList<ITreeViewModelItem> items = new List<ITreeViewModelItem>();


        /// <summary>
        /// Gets all items currently owned by this tree view model, including 
        /// sub-items below the first layer.
        /// </summary>
        public IEnumerable<ITreeViewModelItem> Items => items;


        /// <summary>
        /// Gets the values of all the items of this tree view model (including items below the first layer).
        /// </summary>
        public IEnumerable<object> Values => Items.Select(o => o.Value);

        
        /// <summary>
        /// Represents a collection storing all currently active direct children of this tree view model.
        /// </summary>
        private readonly IList<ITreeViewModelItem> firstLevelItems = new List<ITreeViewModelItem>();


        /// <summary>
        /// Gets all currently active direct children of this tree view model.
        /// </summary>
        public IEnumerable<ITreeViewModelItem> FirstLevelItems => firstLevelItems;


        /// <summary>
        /// Gets or sets a value indicating if the children of this tree view model have been initialized.
        /// The children have lazy loading, therefore, they will not be created until called upon : the first time the Children property getter is called,
        /// or when InvalidateChildren is called.
        /// </summary>
        public bool AreChildrenInitialized { get; private set; }


        //// <summary>
        /// Returns the currently active children, if initialized. Otherwise, it creates and initializes a new set of children. <para/>
        /// IMPORTANT : This is the tree view item's ItemsSource binding property. All children of this tree view model item have lazy loading, 
        /// meaning they will be created and initialized the first time this property's getter is called, or when InvalidateChildren is called.
        /// </summary>
        public IEnumerable<ITreeViewModelItem> Children
        {
            get
            {
                if(!AreChildrenInitialized) {
                    InvalidateChildrenInternal();
                    AreChildrenInitialized = true;
                }

                return FirstLevelItems.ToList();
            }
        }


        /// <summary>
        /// Returns the currently selected tree view model item. <para/>
        /// If no item or more than one item is selected, this property will be null. 
        /// </summary>
        public ITreeViewModelItem SelectedItem => SelectionBinding.SelectedItem;


        /// <summary>
        /// Returns the currently selected tree view model items (this can include items beyond the first layer).
        /// </summary>
        public IEnumerable<ITreeViewModelItem> SelectedItems => SelectionBinding.SelectedItems;
        

        /// <summary>
        /// Gets the value of the currently selected item.
        /// If no item or more than one item is selected, this property will be null.
        /// </summary>
        public object SelectedValue => SelectedItem?.Value;


        /// <summary>
        /// Returns the values of the currently selected items (this can include values of items beyond the first layer).
        /// </summary>
        public IEnumerable<object> SelectedValues => SelectedItems.Select(o => o.Value);


        /// <summary>
        /// Gets all items currently owned by this tree view model, including 
        /// sub-items below the first layer.
        /// </summary>
        IEnumerable<IViewModelItem> IViewModelOwner.Items => Items;


        /// <summary>
        /// Gets the currently selected tree view model item. <para/>
        /// If no item or more than one item is selected, this property will be null. 
        /// </summary>
        IViewModelItem IViewModelOwner.SelectedItem => SelectedItem;
        

        /// <summary>
        /// Returns the currently selected tree view model items (this can include items beyond the first layer).
        /// </summary>
        IEnumerable<IViewModelItem> IViewModelOwner.SelectedItems => SelectedItems;
        

        /// <summary>
        /// Creates a new TreeViewModel instance.
        /// </summary>
        /// <param name="initSvc"></param>
        public TreeViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }


        /// <summary>
        /// Returns the children of this tree view model.
        /// This method must be overriden in the actual view model implementation and is responsible 
        /// for providing the actual content(children) of the tree view model.
        /// The sub-items of the direct children will be provided by the customizable children getter delegates 
        /// owned by the tree view model items.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<ITreeViewModelItem> CreateContentItems();


        /// <summary>
        /// Invalidates the entire content of this tree view model, updating the UI.
        /// </summary>
        public void InvalidateChildren()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                ExpansionRetainer?.CleanUp();
                InvalidateChildrenInternal();
                RaisePropertyChanged(nameof(Children));
            }));
        }
        
        /// <summary>
        /// Invalidates the eitre content of this tree view model internally, without updating the UI.
        /// </summary>
        private void InvalidateChildrenInternal()
        {
            if (!HasBoundSelection) {
                SetDefaultSelectionBinding();
            }

            foreach (var item in FirstLevelItems) {
                item.TearDown();
            }

            firstLevelItems.Clear();

            foreach (var item in CreateContentItems()) {
                item.Initialize(this, null);
                firstLevelItems.Add(item);
            }
        }


        /*******************************************************
         ***************** Item Notifications ******************
         *******************************************************/


        /// <summary>
        /// Notifies this tree view model that a new item has been created as part of it's logical descendants.
        /// </summary>
        /// <param name="item"></param>
        public void NotifyItemCreated(ITreeViewModelItem item)
        {
            SelectionBinding.OnItemCreated(item);
            items.Add(item);
        }

        
        /// <summary>
        /// Notifies this tree view model that an item has been removed from it's logical descendants.
        /// </summary>
        /// <param name="item"></param>
        public void NotifyItemDisposed(ITreeViewModelItem item)
        {
            SelectionBinding.OnItemDisposed(item);
            items.Remove(item);
        }


        /// <summary>
        /// Notifies this tree view model that the selection state of an item which is part of it's logical descendants has changed.
        /// </summary>
        /// <param name="item"></param>
        public void NotifyItemSelectionChanged(ITreeViewModelItem item)
        {
            if (!HasBoundSelection) {
                SetDefaultSelectionBinding();
            }

            SelectionBinding.OnItemSelectionStatusChanged(item);
        }



        /******************************************************
         ************* Expansion State Caching ****************
         ******************************************************/
         
        private TreeExpansionRetainingStrategy expansionRetainingStrategy;
        /// <summary>
        /// Gets or sets the strategy type used to store this tree view model item's expansion states.
        /// </summary>
        public TreeExpansionRetainingStrategy ExpansionRetainingStrategy
        {
            get { return expansionRetainingStrategy; }
            protected set
            {
                expansionRetainingStrategy = value;
                switch(expansionRetainingStrategy)
                {
                    case TreeExpansionRetainingStrategy.ItemPath:       ExpansionRetainer = new ItemPathExpansionRetainer(this); break;
                    case TreeExpansionRetainingStrategy.ReferencePath:  ExpansionRetainer = new ReferencePathExpansionRetainer(this); break;
                    default:
                    {
                        ExpansionRetainer = null;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Represents a context-dependant state retainer, used to cache-in item expansion values and 
        /// determine the expansion state of items after content invalidation operations.
        /// </summary>
        public ITreeExpansionRetainer ExpansionRetainer { get; private set; }


        /// <summary>
        /// Sets the specified custom expansion-state retainer to be used for caching this tree view model's content expansion states. <para/>
        /// NOTE : In order to set set a custom expansion retainer, the expansion retaining strategy must be set to custom strategy.
        /// </summary>
        /// <param name="expansionRetainer"></param>
        protected void SetCustomExpansionRetainer(ITreeExpansionRetainer expansionRetainer)
        {
            if(ExpansionRetainingStrategy != TreeExpansionRetainingStrategy.CustomStrategy)
            {
                throw new Exception("Error : Cannot set a custom expansion retainer unless the expansion retaining strategy is set to custom.");
            }

            ExpansionRetainer = expansionRetainer;
        }



        /******************************************************
         ***************** Selection Handling  ****************
         ******************************************************/

        /// <summary>
        /// Gets or sets the currently active selection binding that is used to handle all items.
        /// </summary>
        public ITreeSelectionBinding SelectionBinding { get; private set; }


        /// <summary>
        /// Gets a value indicating whether this tree view model has a selection binding set on it's content. 
        /// </summary>
        public bool HasBoundSelection { get { return SelectionBinding != null; } }


        private bool syncItems;
        /// <summary>
        /// Gets or sets a value indicating if, in the case of multiple selection, the items should be synced by value.
        /// If true, when an item is selected in the tree, all items having the same value will automatically be selected.
        /// </summary>
        protected bool SyncItems
        {
            get { return syncItems; }
            set
            {
                if (syncItems != value)
                {
                    syncItems = value;
                    if (HasBoundSelection)
                    {
                        SelectionBinding.SyncItems = value;
                    }
                }
            }
        }


        /// <summary>
        /// Sets a two-way binding between the value of the selected view model item and the specified selection. <para/>
        /// When the specified selection changes, the selected tree view model item will be adapted.
        /// Likewise, when the selection states of the tree view model item changes, the selection will be adapted. <para/>
        /// NOTE : If this tree view model already has a bound selection, then the selection binding must be broken first, otherwise an exception will be thrown. <para/>
        /// NOTE : A single selection binding can be set only if this tree view model item does not allow multiple selection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selection"></param>
        protected void SetSelectionBinding<T>(SingleSelection<T> selection)
        {
            selection.AssertParameterNotNull(nameof(selection));
            if (AllowMultipleSelection)
            {
                throw new Exception("Error : Attempting to set a selection binding between a single selection and a tree view model which doesn't support multiple selection.");
            }

            if (HasBoundSelection)
            {
                throw new Exception("Error : Cannot set a selection binding on this tree view model instance because one already exists. Break the current selection binding before setting a new one.");
            }

            SelectionBinding = new SingleTreeSelectionBinding<T>(selection);
            SelectionBinding.Initialize(this);
            SelectionBinding.SyncItems = SyncItems;
        }


        /// <summary>
        /// Sets a two-way binding between the values of the selected tree view model items and the specified multiple selection. <para/>
        /// When the specified selection changes, the selection states of the view model items will be adapted.
        /// Likewise, when the selection states of the view model items change, the selection will be adapted. <para/>
        /// NOTE : If this tree view model already has a bound selection, then the selection binding must be broken first, otherwise an exception will be thrown. <para/>
        /// NOTE : A single selection binding can be set only if this tree view model item does not allow multiple selection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selection"></param>
        protected void SetSelectionBinding<T>(MultipleSelection<T> selection)
        {
            selection.AssertParameterNotNull(nameof(selection));

            if (!AllowMultipleSelection)
            {
                throw new Exception("Error : Attempting to set a selection binding between a multiple selection and a list of view model which doesn't support multiple selection.");
            }

            if (HasBoundSelection)
            {
                throw new Exception("Error : Cannot set a selection binding on this list view model instance because one already exists. Break the current selection binding before setting a new one.");
            }

            SelectionBinding = new MultipleTreeSelectionBinding<T>(selection);
            SelectionBinding.Initialize(this);
            SelectionBinding.SyncItems = SyncItems;
        }


        /// <summary>
        /// Breaks the current selection binding. <para/>
        /// NOTE : If there is no active selection binding, an exception will be thrown.
        /// </summary>
        protected void BreakSelectionBinding()
        {
            if (!HasBoundSelection)
            {
                throw new Exception("Error : Attempting to break a selection binding on a list view model that is not bound to any selection.");
            }

            SelectionBinding.TearDown();
            SelectionBinding = null;
        }


        /// <summary>
        /// Sets a default selection binding in accordance with the selection mode.
        /// </summary>
        private void SetDefaultSelectionBinding()
        {
            if (AllowMultipleSelection)
            {
                SetSelectionBinding(new MultipleViewModelItemSelection());
            }

            else
            {
                SetSelectionBinding(new SingleViewModelItemSelection());
            }
        }



        // ------------------------------------------------------------------------------------ //

        /// <summary>
        /// Tears down this tree view model instance, breaking any existing selection binding and tearing down all associated items.
        /// </summary>
        public override void TearDown()
        {
            BreakSelectionBinding();
            foreach(var item in FirstLevelItems)
            {
                item.TearDown();
            }

            firstLevelItems.Clear();
            ExpansionRetainer = null;

            base.TearDown();
        }
    }
}

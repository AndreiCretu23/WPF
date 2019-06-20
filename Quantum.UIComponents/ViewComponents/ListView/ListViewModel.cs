using Quantum.Events;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents the base data context model for a list component. 
    /// </summary>
    public abstract class ListViewModel : ViewModelBase, IListViewModel
    {
        
        [Service]
        public IIconManagerService IconManager { get; set; }
        
        private bool allowMultipleSelection = true;
        /// <summary>
        /// Gets or sets a value indicating if multiple selection is allowed in the context of this list view model.
        /// NOTE : Setting this property breaks the current selection binding (if any).
        /// </summary>
        public bool AllowMultipleSelection
        {
            get { return allowMultipleSelection; }
            protected set
            {
                if(allowMultipleSelection != value)
                {
                    allowMultipleSelection = value;
                    if(HasBoundSelection) {
                        BreakSelectionBinding();
                    }

                    RaisePropertyChanged(nameof(AllowMultipleSelection));
                }
            }
        }
        

        /// <summary>
        /// Represents an internal collection storing all currently active items.
        /// </summary>
        private readonly IList<IListViewModelItem> items = new List<IListViewModelItem>();


        /// <summary>
        /// Returns all items owned by this list view model.
        /// </summary>
        public IEnumerable<IListViewModelItem> Items => items;
        

        /// <summary>
        /// Creates and initializes a new set of children provided by the extendable content-creation method.
        /// Also, it disposes the previously created items, releasing all associated resources. <para/>
        /// IMPORTANT : This property is only used for binding and should never be accessed in the code. To get the currently active children, use the "Items" property.
        /// </summary>
        public IEnumerable<IListViewModelItem> Children
        {
            get
            {
                if(!HasBoundSelection) {
                    SetDefaultSelectionBinding();
                }

                foreach (var item in Items) {
                    PropertyChangedEventManager.RemoveHandler(item, ItemSelectionChanged, nameof(item.IsSelected));
                    item.TearDown();
                }

                items.Clear();
                
                foreach(var item in CreateContentItems()) {
                    item.Initialize(this);
                    item.IsSelected = SelectionBinding.IsContainedInSelection(item);
                    PropertyChangedEventManager.AddHandler(item, ItemSelectionChanged, nameof(item.IsSelected));
                    
                    items.Add(item);
                }

                SelectionBinding.OnItemsChanging(Items);
                return Items.ToList();
            }
        }
        

        /// <summary>
        /// Returns the values of the current items.
        /// </summary>
        public IEnumerable<object> Values => Items.Select(o => o.Value);


        /// <summary>
        /// Returns the currently selected item. <para/>
        /// If no item or more than one item is selected, this property will be null. 
        /// </summary>
        public IListViewModelItem SelectedItem => SelectedItems.IsSingleElement() ? SelectedItems.Single() : null;


        /// <summary>
        /// Returns the currently selected items.
        /// </summary>
        public IEnumerable<IListViewModelItem> SelectedItems => Items.Where(o => o.IsSelected);


        /// <summary>
        /// Returns the value of the currently selected item.
        /// If no item or more than one item is selected, this property will be null.
        /// </summary>
        public object SelectedValue => SelectedItem?.Value;

        
        /// <summary>
        /// Returns the values of the currently selected items.
        /// </summary>
        public IEnumerable<object> SelectedValues => SelectedItems.Select(o => o.Value);


        /// <summary>
        /// Returns all items owned by this view model owner.
        /// </summary>
        IEnumerable<IViewModelItem> IViewModelOwner.Items => Items;


        /// <summary>
        /// Returns the currently selected item in the context of this view model owner. <para/>
        /// If no item or more than one item is selected, this property will be null. 
        /// </summary>
        IViewModelItem IViewModelOwner.SelectedItem => SelectedItem;


        /// <summary>
        /// Returns the currently selected items in the context of this view model owner.
        /// </summary>
        IEnumerable<IViewModelItem> IViewModelOwner.SelectedItems => SelectedItems;
        

        /// <summary>
        /// Creates a new ListViewModel instance.
        /// </summary>
        /// <param name="initSvc"></param>
        public ListViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }


        /// <summary>
        /// Returns the children of this list view model.
        /// This method mult be overriden in the actual view model implementation and is responsible 
        /// for providing the actual content(children) of the list view model.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<IListViewModelItem> CreateContentItems();


        /// <summary>
        /// Invalidates this children of this list view model, updating the UI.
        /// </summary>
        public void InvalidateChildren()
        {
            RaisePropertyChanged(nameof(Children));
        }







        /**************************************************************
         ******************** Selection Handling **********************
         **************************************************************/

        /// <summary>
        /// Gets or sets the currently active selection binding that is used to handle all items.
        /// </summary>
        private IListSelectionBinding SelectionBinding { get; set; }
        

        /// <summary>
        /// Gets a value indicating whether this list view model has a selection binding set on it's content. 
        /// </summary>
        public bool HasBoundSelection { get { return SelectionBinding != null; } }


        private bool syncItems;
        /// <summary>
        /// Gets or sets a value indicating if, in the case of multiple selection, the items should be synced by value.
        /// If true, when an item is selected in the list, all items having the same value will automatically be selected.
        /// </summary>
        protected bool SyncItems
        {
            get { return syncItems; }
            set
            {
                if(syncItems != value)
                {
                    syncItems = value;
                    if(HasBoundSelection)
                    {
                        SelectionBinding.SyncItems = value;
                    }
                }
            }
        }
        

        /// <summary>
        /// Sets a two-way binding between the value of the selected view model item and the specified selection. <para/>
        /// When the specified selection changes, the selected view model item will be adapted.
        /// Likewise, when the selection states of the view model item changes, the selection will be adapted. <para/>
        /// NOTE : If this list view model already has a bound selection, then the selection binding must be broken first, otherwise an exception will be thrown. <para/>
        /// NOTE : A single selection binding can be set only if this list view model item does not allow multiple selection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selection"></param>
        protected void SetSelectionBinding<T>(SingleSelection<T> selection)
        {
            selection.AssertParameterNotNull(nameof(selection));
            if(AllowMultipleSelection) {
                throw new Exception("Error : Attempting to set a selection binding between a single selection and a list view model which doesn't support multiple selection.");
            }

            if(HasBoundSelection) {
                throw new Exception("Error : Cannot set a selection binding on this list view model instance because one already exists. Break the current selection binding before setting a new one.");
            }

            SelectionBinding = new SingleListSelectionBinding<T>(selection);
            SelectionBinding.Initialize(this);
            SelectionBinding.SyncItems = SyncItems;
        }


        /// <summary>
        /// Sets a two-way binding between the values of the selected view model items and the specified multiple selection. <para/>
        /// When the specified selection changes, the selection states of the view model items will be adapted.
        /// Likewise, when the selection states of the view model items change, the selection will be adapted. <para/>
        /// NOTE : If this list view model already has a bound selection, then the selection binding must be broken first, otherwise an exception will be thrown. <para/>
        /// NOTE : A single selection binding can be set only if this list view model item does not allow multiple selection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selection"></param>
        protected void SetSelectionBinding<T>(MultipleSelection<T> selection)
        {
            selection.AssertParameterNotNull(nameof(selection));

            if(!AllowMultipleSelection) {
                throw new Exception("Error : Attempting to set a selection binding between a multiple selection and a list of view model which doesn't support multiple selection.");
            }

            if (HasBoundSelection) {
                throw new Exception("Error : Cannot set a selection binding on this list view model instance because one already exists. Break the current selection binding before setting a new one.");
            }

            SelectionBinding = new MultipleListSelectionBinding<T>(selection);
            SelectionBinding.Initialize(this);
            SelectionBinding.SyncItems = SyncItems;
        }

        
        /// <summary>
        /// Breaks the current selection binding. <para/>
        /// NOTE : If there is no active selection binding, an exception will be thrown.
        /// </summary>
        protected void BreakSelectionBinding()
        {
            if(!HasBoundSelection) {
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
            if(AllowMultipleSelection)
            {
                SetSelectionBinding(new MultipleViewModelItemSelection());
            }

            else
            {
                SetSelectionBinding(new SingleViewModelItemSelection());
            }
        }

        
        /// <summary>
        /// Handles the item selection changed event for all the current items.
        /// Initializes a default selection binding in accordance with the current selection mode if there is none and forwards the selected item to it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            if(!HasBoundSelection) {
                SetDefaultSelectionBinding();
            }

            SelectionBinding.OnItemSelectionStatusChanged((IListViewModelItem)sender);
        }


        // ------------------------------------------------------------------------------------ //

        /// <summary>
        /// Tears down this list view model instance, breaking any existing selection binding and tearing down all associated items.
        /// </summary>
        public override void TearDown()
        {
            if(HasBoundSelection) {
                BreakSelectionBinding();
            }

            foreach(var item in Items) {
                PropertyChangedEventManager.RemoveHandler(item, ItemSelectionChanged, nameof(item.IsSelected));
                item.TearDown();
            }

            items.Clear();

            base.TearDown();
        }

    }
}

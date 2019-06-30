using Quantum.UIComposition;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents the basic data context model of a tree view item.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with this tree view model item.</typeparam>
    public class TreeViewModelItem<T> : ObservableObject, ITreeViewModelItem
    {
        
        /// <summary>
        /// Gets or sets the value associated with this tree view model item.
        /// </summary>
        public T Value { get; private set; }


        /// <summary>
        /// Gets the type of the value this tree view model item is associated with.
        /// </summary>
        public Type ValueType { get { return typeof(T); } }


        /// <summary>
        /// Gets or sets the owner of this tree view model item instance.
        /// The owner of a tree view model item is the tree view model root.
        /// </summary>
        public ITreeViewModel Owner { get; private set; }


        /// <summary>
        /// Gets or sets the parent of this tree view model item instance.
        /// NOTE : If this tree view model item instance is a direct child of it's owner, this property will be null.
        /// </summary>
        public ITreeViewModelItem Parent { get; private set; }


        /// <summary>
        /// Represents a collection storing all currently active direct children of this tree view model item.
        /// </summary>
        private readonly IList<ITreeViewModelItem> firstLevelItems = new List<ITreeViewModelItem>();


        /// <summary>
        /// Gets all the currently active direct children of this tree view model item.
        /// </summary>
        public IEnumerable<ITreeViewModelItem> FirstLevelItems => firstLevelItems;


        /// <summary>
        /// Gets or sets a value indicating if this tree view model item instance has been initialized in the context of an owner and a parent.
        /// </summary>
        public bool IsInitialized { get; private set; }


        private bool isSelected = false;
        /// <summary>
        /// Gets or sets a value indicating whether this tree view model item is selected or not.
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if(isSelected != value)
                {
                    isSelected = value;
                    Owner.NotifyItemSelectionChanged(this);
                    RaisePropertyChanged(nameof(IsSelected));
                }
            }
        }


        private bool isExpanded;
        /// <summary>
        /// Gets or sets a value indicating whether this tree view model item is expanded or not.
        /// </summary>
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                RaisePropertyChanged(nameof(IsExpanded));
            }
        }

        /// <summary>
        /// Gets or sets a value-dependant lambda expression used to determine the header of this tree view model item.
        /// </summary>
        public Func<T, string> HeaderGetter { get; set; }


        /// <summary>
        /// Gets or sets a value-dependant lambda expression used to determine the resource-path of the icon of this tree view model item.
        /// </summary>
        public Func<T, string> IconGetter { get; set; }


        /// <summary>
        /// Gets or sets the URI of the fallback icon, in case the tree view model item does not have a value-dependant icon getter.
        /// </summary>
        public string FallbackIcon { get; set; }


        /// <summary>
        /// Gets or sets a delegate used to determine the children of this tree view model item.
        /// </summary>
        public Func<T, IEnumerable<ITreeViewModelItem>> ChildrenGetter { get; set; }


        /// <summary>
        /// Gets the header of this tree view model item instance provided by the value-dependant header provider lambda expression.
        /// </summary>
        public string Header { get { return HeaderGetter?.Invoke(Value) ?? typeof(T).Name; } }


        /// <summary>
        /// Gets the Uri path of the icon associated with this tree view model item provided by the value-dependant icon provider lambda expression.
        /// </summary>
        public string Icon { get { return IconGetter?.Invoke(Value) ?? FallbackIcon; } }


        // <summary>
        /// Gets a value indicating if the children of this tree view model item have been initialized.
        /// The children have lazy loading, therefore, they will not be created until called upon : the first time the Children property getter is called.
        /// </summary>
        public bool AreChildrenInitialized { get; private set; }


        /// <summary>
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

                return firstLevelItems.ToList();
            }
        }


        /// <summary>
        /// Gets the value associated with this tree view model item instance.
        /// </summary>
        object IViewModelItem.Value => Value;


        /// <summary>
        /// Gets the owner of this tree view model item instance.
        /// </summary>
        IViewModelOwner IViewModelItem.Owner => Owner;


        /// <summary>
        /// Creates a new tree view model instance having the associated value.
        /// </summary>
        /// <param name="value"></param>
        public TreeViewModelItem(T value)
        {
            Value = value;
        }


        /******************************************************************************************************
         ****************************************** Item Management *******************************************
         ******************************************************************************************************/


        /// <summary>
        /// Initializes this tree view model item instance in the context of the specified owner and parent.
        /// </summary>
        /// <param name="owner">The owner of this tree view model item.</param>
        /// <param name="parent">The tree view model item parent of this instance, or null if this tree view model item is a direct child of it's owner.</param>
        public void Initialize(ITreeViewModel owner, ITreeViewModelItem parent)
        {
            owner.AssertParameterNotNull(nameof(owner));
            
            if(IsInitialized)
            {
                throw new Exception("Error : Attempting to initialize a tree view model item instance that has already been initialized.");
            }

            Owner = owner;
            Parent = parent;
            
            if(FallbackIcon == null) {
                FallbackIcon = Owner.IconManager.GetIconForType<T>();
            }

            if(Owner.ExpansionRetainingStrategy != TreeExpansionRetainingStrategy.None) {
                if(Owner.ExpansionRetainer == null) {
                    throw new Exception("Error : The owner of this tree view model item doesn't have an expansion retainer set while having a defined expansion retaining strategy.");
                }

                Owner.ExpansionRetainer.LoadExpansionState(this);
            }
            
            Owner.NotifyItemCreated(this);

            IsInitialized = true;
        }


        /// <summary>
        /// Tears down this tree view model item instance and all of it's descendants, releasing all associated references / resources.
        /// </summary>
        public void TearDown()
        {
            foreach(var item in FirstLevelItems) {
                item.TearDown();
            }

            firstLevelItems.Clear();

            if(Owner.ExpansionRetainingStrategy != TreeExpansionRetainingStrategy.None) {
                if (Owner.ExpansionRetainer == null) {
                    throw new Exception("Error : The owner of this tree view model item doesn't have an expansion retainer set while having a defined expansion retaining strategy.");
                }

                Owner.ExpansionRetainer.SaveExpansionState(this);
            }
            
            Owner.NotifyItemDisposed(this);

            Value = default;
            HeaderGetter = null;
            IconGetter = null;
            Owner = null;
            Parent = null;
            FallbackIcon = null;
        }



        /******************************************************************************************************
         ******************************************* Invalidation *********************************************
         ******************************************************************************************************/

        /// <summary>
        /// Invalidates the sub-items of this tree view model item, updating the UI.
        /// </summary>
        public void InvalidateChildren()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                Owner.ExpansionRetainer?.CleanUp();
                InvalidateChildrenInternal();
                RaisePropertyChanged(nameof(Children));
            }));
        }

        /// <summary>
        /// Invalidates the sub-items of this tree view model item internally, without updating the UI.
        /// </summary>
        private void InvalidateChildrenInternal()
        {
            foreach (var item in FirstLevelItems) {
                item.TearDown();
            }

            firstLevelItems.Clear();
            foreach (var item in (ChildrenGetter?.Invoke(Value) ?? Enumerable.Empty<ITreeViewModelItem>())) {
                item.Initialize(Owner, this);
                firstLevelItems.Add(item);
            }
        }



        /******************************************************************************************************
         ********************************************* Utilities **********************************************
         ******************************************************************************************************/

        /// <summary>
        /// Creates a path representation of this tree view model item using the headers of all the items up to the root.
        /// </summary>
        /// <param name="separator">The separator to use when joining the components of the resulted path.</param>
        /// <returns></returns>
        public string CreatePath(string separator = "~")
        {
            return string.Join(separator, GetThisAndAncestors().Reverse().Select(o => o.Header));
        }


        /// <summary>
        /// Returns a collection containined all ancestors of this tree view model item that satisfy the specified condition (if given).
        /// </summary>
        /// <param name="condition">The (optional) condition that a tree view model item must satisfy in order to be included in the result.</param>
        /// <returns></returns>
        public IEnumerable<ITreeViewModelItem> GetAncestors(Predicate<ITreeViewModelItem> condition = null)
        {
            var result = new List<ITreeViewModelItem>();
            condition = condition ?? (o => true);

            var parent = Parent;
            while(parent != null)
            {
                if(condition(parent)) {
                    result.Add(parent);
                }

                parent = parent.Parent;
            }

            return result;
        }


        /// <summary>
        /// Returns a collection containing this instance and all ancestors of this tree view model item that satisfy the specified condition (if given).
        /// </summary>
        /// <param name="condition">The (optional) condition that a tree view model item must satisfy in order to be included in the result.</param>
        /// <returns></returns>
        public IEnumerable<ITreeViewModelItem> GetThisAndAncestors(Predicate<ITreeViewModelItem> condition = null)
        {
            var result = new List<ITreeViewModelItem>();
            condition = condition ?? (o => true);

            ITreeViewModelItem parent = this;
            while (parent != null)
            {
                if (condition(parent))
                {
                    result.Add(parent);
                }

                parent = parent.Parent;
            }

            return result;
        }


        /// <summary>
        /// Returns a collection containing all currently active descendants of this tree view model item that satisfy the specified condition (if given).
        /// </summary>
        /// <param name="condition">The (optional) condition that a tree view model item must satisfy in order to be included in the result.</param>
        /// <returns></returns>
        public IEnumerable<ITreeViewModelItem> GetDescendants(Predicate<ITreeViewModelItem> condition = null)
        {
            var result = new List<ITreeViewModelItem>();
            condition = condition ?? (o => true);

            foreach(var item in FirstLevelItems)
            {
                if(condition(item)) {
                    result.Add(item);
                }

                result.AddRange(item.GetDescendants(condition));
            }

            return result;
        }


        /// <summary>
        /// Returns a collection containing this instance and all currently active descendants of this tree view model item that satisfy the specified condition (if given).
        /// </summary>
        /// <param name="condition">The (optional) condition that a tree view model item must satisfy in order to be included in the result.</param>
        /// <returns></returns>
        public IEnumerable<ITreeViewModelItem> GetThisAndDescendants(Predicate<ITreeViewModelItem> condition)
        {
            var result = new List<ITreeViewModelItem>();
            condition = condition ?? (o => true);

            result.AddRange(GetDescendants(condition));
            if(condition(this)) {
                result.Add(this);
            }

            return result;
        }
        
    }
}

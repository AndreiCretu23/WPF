using Quantum.UIComposition;
using Quantum.Utils;
using System;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents the basic data context model of a list item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListViewModelItem<T> : ObservableObject, IListViewModelItem
    {
        
        /// <summary>
        /// Gets or sets the value associated with this list view model item instance.
        /// </summary>
        public T Value { get; private set; }
        
        
        /// <summary>
        /// Gets the 
        /// </summary>
        public Type ValueType { get { return typeof(T); } }

        
        /// <summary>
        /// Gets or sets the owner of this list view model item instance.
        /// </summary>
        public IListViewModel Owner { get; private set; }


        /// <summary>
        /// Gets or sets a value indicating if this list view model item has been initialized in the context of an owner.
        /// </summary>
        public bool IsInitialized { get; private set; }


        private bool isSelected = false;
        /// <summary>
        /// A value indicating whether this ListViewModelItem is selected or not.
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if(isSelected != value)
                {
                    isSelected = value;
                    RaisePropertyChanged(nameof(IsSelected));
                }
            }
        }
        

        /// <summary>
        /// Gets the header of this ListViewModelItem instance provided by the header provider lambda expression.
        /// </summary>
        public string Header { get { return HeaderGetter?.Invoke(Value) ?? typeof(T).Name; } }
        

        /// <summary>
        /// Gets the Uri path of the icon associated with this list view model item provided by the icon provider lambda expression.
        /// </summary>
        public string Icon { get { return IconGetter?.Invoke(Value) ?? FallbackIcon; } }


        /// <summary>
        /// Gets or sets a value-dependent lambda expression used to determine the header of this list view model item.
        /// </summary>
        public Func<T, string> HeaderGetter { get; set; }
        

        /// <summary>
        /// Gets or sets a value dependants lambda expression used to determine the header of this list view model item.
        /// </summary>
        public Func<T, string> IconGetter { get; set; }


        /// <summary>
        /// Gets or sets the URI of the fallback icon, in case the list view model item does not have a value-dependant icon getter.
        /// </summary>
        public string FallbackIcon { get; set; }
        

        /// <summary>
        /// Gets the value associated with this ListViewModelItem instance.
        /// </summary>
        object IViewModelItem.Value => Value;


        /// <summary>
        /// Gets the owner of this ListViewModelItem instance.
        /// </summary>
        IViewModelOwner IViewModelItem.Owner => Owner;
        

        /// <summary>
        /// Creates a new ListViewModelItem instance.
        /// </summary>
        /// <param name="value"></param>
        public ListViewModelItem(T value)
        {
            Value = value;
        }


        /// <summary>
        /// Initializes this list view model item in the context of the specified owner.
        /// </summary>
        /// <param name="owner"></param>
        public void Initialize(IListViewModel owner)
        {
            owner.AssertParameterNotNull(nameof(owner));

            if(IsInitialized)
            {
                throw new Exception("Error : Attempting to initialize a list view model item instance that has already been initialized.");
            }

            Owner = owner;
            FallbackIcon = Owner.IconManager.GetIconForType<T>();
            IsInitialized = true;
        }

        /// <summary>
        /// Tears down this ListViewModelItem instance and all associated references / resources.
        /// </summary>
        public void TearDown()
        {
            Value = default;
            HeaderGetter = null;
            IconGetter = null;
            Owner = null;
            FallbackIcon = null;
        }
    }
}

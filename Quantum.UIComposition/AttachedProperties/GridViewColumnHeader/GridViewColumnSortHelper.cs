using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UIGridViewColumnHeader = System.Windows.Controls.GridViewColumnHeader;
using UIItemsControl = System.Windows.Controls.ItemsControl;

namespace Quantum.AttachedProperties
{
    internal static class GridViewColumnSortHelper
    {
        /// <summary>
        /// This property is responsible for storing all the GridViewColumnHeaders that have sorting enabled 
        /// in the context of their common ItemsControl ancestor.
        /// </summary>
        private static readonly DependencyPropertyKey SortableColumnsPropertyKey = DependencyProperty.RegisterAttachedReadOnly
        (
            name : "SortableColumns",
            propertyType: typeof(IList<UIGridViewColumnHeader>),
            ownerType: typeof(GridViewColumnSortHelper),
            defaultMetadata: new PropertyMetadata(defaultValue: new List<UIGridViewColumnHeader>())
        );

        private static IList<UIGridViewColumnHeader> GetSortableColumns(DependencyObject dependencyObject)
        {
            return (IList<UIGridViewColumnHeader>)dependencyObject.GetValue(SortableColumnsPropertyKey.DependencyProperty);
        }
        
        [SuppressMessage(category: "Microsoft.Performance", checkId: "CS0067", Justification = "GridViewColumnSortCommand needs to implement ICommand.")]
        internal class GridViewColumnSortCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            private UIGridViewColumnHeader Header { get; }
            private UIItemsControl ItemsControl { get; set; }
            private bool IsInitialized { get; set; }

            public GridViewColumnSortCommand(UIGridViewColumnHeader header)
            {
                Header = header;
            }

            public void Initialize()
            {
                if(IsInitialized) {
                    throw new Exception("Error : Attempting to initialize an already-initialized GridViewColumnSortCommand.");
                }

                ItemsControl = Header.GetVisualAncestorsOfType<UIItemsControl>().First();
                if(GetSortableColumns(ItemsControl).Contains(Header)) {
                    throw new Exception("Error : Attempting to add multiple sortable column handlers to a ItemsControl for the same GridViewColumnHeader.");
                }

                GetSortableColumns(ItemsControl).Add(Header);
                Header.SetValue(GridViewColumnHeader.IsSortablePropertyKey, true);
                IsInitialized = true;
            }

            public void Teardown()
            {
                if(!IsInitialized) {
                    throw new Exception("Error : Attempting to teardown a non-initialized GridViewColumnSortCommand.");
                }

                if(!GetSortableColumns(ItemsControl).Contains(Header)) {
                    throw new Exception("Error : Attempting to teardown a sortable column command handler that has not been registered.");
                }

                GetSortableColumns(ItemsControl).Remove(Header);
                Header.SetValue(GridViewColumnHeader.IsSortablePropertyKey, false);
                IsInitialized = false;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if(!IsInitialized)
                {
                    throw new Exception("Error : Attepting to execute a GridViewColumn sort operation using an uninitialized sort command.");
                }

                foreach(var header in GetSortableColumns(ItemsControl))
                {
                    if(header != Header)
                    {
                        header.SetValue(GridViewColumnHeader.SortOrderPropertyKey, GridViewColumnSortOrder.None);
                    }
                }

                var oldSortOrder = GridViewColumnHeader.GetSortOrder(Header);
                var newSortOrder = oldSortOrder == GridViewColumnSortOrder.None ||
                                   oldSortOrder == GridViewColumnSortOrder.Descending ? GridViewColumnSortOrder.Ascending : 
                                                                                        GridViewColumnSortOrder.Descending;

                Header.SetValue(GridViewColumnHeader.SortOrderPropertyKey, newSortOrder);
                var sortKey = GridViewColumnHeader.GetSortKey(Header);
                ItemsControl.Items.SortDescriptions.Clear();
                ItemsControl.Items.SortDescriptions.Add
                (
                    new SortDescription
                    (
                        propertyName : sortKey.Name, 
                        direction: newSortOrder == GridViewColumnSortOrder.Ascending ? ListSortDirection.Ascending : 
                                                                                       ListSortDirection.Descending
                    )
                );
            }
        }
    }
}

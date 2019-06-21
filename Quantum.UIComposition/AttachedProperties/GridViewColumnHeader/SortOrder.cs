using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Quantum.AttachedProperties
{
    public enum GridViewColumnSortOrder
    {
        Ascending,
        Descending,
        None
    }

    public static partial class GridViewColumnHeader
    {
        /// <summary>
        /// This property stores the current sort order of a GridViewColumnHeader.
        /// It's value is read-only and managed by the GridViewColumnHeader sorting mechanism.
        /// </summary>
        public static readonly DependencyPropertyKey SortOrderPropertyKey = DependencyProperty.RegisterAttachedReadOnly
        (
            name : "SortOrder",
            propertyType: typeof(GridViewColumnSortOrder),
            ownerType: typeof(GridViewColumnHeader),
            defaultMetadata: new PropertyMetadata(defaultValue: GridViewColumnSortOrder.None)
        );

        public static GridViewColumnSortOrder GetSortOrder(DependencyObject dependencyObject)
        {
            return (GridViewColumnSortOrder)dependencyObject.GetValue(SortOrderPropertyKey.DependencyProperty);
        }
    }
}

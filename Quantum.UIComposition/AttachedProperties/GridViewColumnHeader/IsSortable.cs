using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Quantum.AttachedProperties
{
    public static partial class GridViewColumnHeader
    {
        /// <summary>
        /// This property stores a value indicating whether the parent ItemsControl can be sorted by this GridViewColumnHeader or not.
        /// It's value is read-only and managed by the GridViewColumnHeader sorting mechanism.
        /// </summary>
        public static readonly DependencyPropertyKey IsSortablePropertyKey = DependencyProperty.RegisterAttachedReadOnly
        (
            name : "IsSortable",
            propertyType: typeof(bool),
            ownerType: typeof(GridViewColumnHeader),
            defaultMetadata: new PropertyMetadata(defaultValue: false)
        );


        public static bool GetIsSortable(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsSortablePropertyKey.DependencyProperty);
        }
    }
}

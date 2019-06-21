using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UIFrameworkElement = System.Windows.FrameworkElement;
using UIListView = System.Windows.Controls.ListView;
using UIGridViewColumnHeader = System.Windows.Controls.GridViewColumnHeader;
using Quantum.Utils;
using System.ComponentModel;

namespace Quantum.AttachedProperties
{
    public static partial class GridViewColumnHeader
    {
        /// <summary>
        /// Registers the bound sort key property to a GridViewColumnHeader in the context of the parent ItemsControl.
        /// The actual value of this property must be the property info of the data context item property the items must be sorted by.
        /// Example : 
        /// <ListView.View>
        ///      <GridView>
        ///         <GridViewColumn DisplayMemberBinding = "{Binding Header}" Width="250">
        ///             <GridViewColumn.Header>
        ///                 <GridViewColumnHeader p:GridViewColumnHeader.SortKey="{Binding HeaderSortKey}" Content="Name"/>
        ///             </GridViewColumn.Header>
        ///         </GridViewColumn>
        ///         <GridViewColumn DisplayMemberBinding = "{Binding Description}" Width="250">
        ///             <GridViewColumnHeader Content = "Description" p:GridViewColumnHeader.SortKey="{Binding DescriptionSortKey}"/>
        ///         </GridViewColumn>
        ///     </GridView>
        /// </ListView.View>
        /// 
        /// ViewModel : 
        /// public PropertyInfo HeaderSortKey { get { return typeof(ViewModelItemType).GetProperty("Header"); } }
        /// public PropertyInfo DescriptionSortKey { get { return typeof(ViewModelItemType).GetProperty("Description"); } }
        /// </summary>
        public static readonly DependencyProperty SortKeyProperty = DependencyProperty.RegisterAttached
        (
             name: "SortKey",
             propertyType: typeof(PropertyInfo),
             ownerType: typeof(GridViewColumnHeader),
             defaultMetadata: new PropertyMetadata(defaultValue: null, propertyChangedCallback: OnSortKeyChanged)
        );
        
        public static PropertyInfo GetSortKey(DependencyObject dependencyObject)
        {
            return (PropertyInfo)dependencyObject.GetValue(SortKeyProperty);
        }

        public static void SetSortKey(DependencyObject dependencyObject, object value)
        {
            dependencyObject.SetValue(SortKeyProperty, value);
        }
        
        private static void OnSortKeyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if(!(sender is UIGridViewColumnHeader header)) {
                throw new Exception("Error : The sort key property can only be set on a GridViewColumnHeader.");
            }
            
            if(e.OldValue != null)
            {
                if(header.IsLoaded)
                {
                    Unregister(header);
                }
                else
                {
                    header.Loaded += UnregisterOnLoad;
                }
            }

            if(e.NewValue != null)
            {
                if(header.IsLoaded)
                {
                    Register(header);
                }
                else
                {
                    header.Loaded += RegisterOnLoad;
                }
            }
        }
        
        private static void RegisterOnLoad(object sender, RoutedEventArgs e)
        {
            var header = (UIGridViewColumnHeader)sender;
            Register(header);
            header.Loaded -= RegisterOnLoad;
        }

        private static void UnregisterOnLoad(object sender, RoutedEventArgs e)
        {
            var header = (UIGridViewColumnHeader)sender;
            Unregister(header);
            header.Loaded -= UnregisterOnLoad;
        }

        private static void Register(UIGridViewColumnHeader header)
        {
            var sortCommand = new GridViewColumnSortHelper.GridViewColumnSortCommand(header);
            sortCommand.Initialize();
            header.Command = sortCommand;
        }
        
        private static void Unregister(UIGridViewColumnHeader header)
        {
            var sortCommand = header.Command as GridViewColumnSortHelper.GridViewColumnSortCommand;
            sortCommand.Teardown();
            header.Command = null;
        }
    }
}

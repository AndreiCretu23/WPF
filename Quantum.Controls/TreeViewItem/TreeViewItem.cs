using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Quantum.Utils;

namespace Quantum.Controls
{
    public class TreeViewItem : System.Windows.Controls.ItemsControl
    {
        #region DependencyProperties

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register
        (
            name: "IsExpanded",
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty IsCheckableProperty = DependencyProperty.Register
        (
            name: "IsCheckable",
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register
        (
            name: "IsChecked",
            propertyType: typeof(bool),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register
        (
            name: "Icon",
            propertyType: typeof(ImageSource), 
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(null)
        );

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register
        (
            name: "Header",
            propertyType: typeof(string),
            ownerType: typeof(TreeViewItem),
            typeMetadata: new PropertyMetadata(null)
        );

        #endregion DependencyProperties

        
        #region Properties

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public bool IsCheckable
        {
            get { return (bool)GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }


        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        
        #endregion Properties


        public TreeViewItem()
        {
        }

        static TreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(typeof(TreeViewItem)));
        }

        protected override bool ShouldApplyItemContainerStyle(DependencyObject container, object item)
        {
            return container is TreeViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItem();
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Quantum.Controls
{
    public class ToolBarContainer : ToolBarTray
    {
        #region Properties

        public static readonly DependencyProperty ToolBarStyleProperty = DependencyProperty.Register
            ("ToolBarStyle", typeof(Style), typeof(ToolBarContainer), new PropertyMetadata(null));

        public static readonly DependencyProperty ToolBarItemsSourceProperty = DependencyProperty.Register
            ("ToolBarItemsSource", typeof(IEnumerable<object>), typeof(ToolBarContainer), new PropertyMetadata(Enumerable.Empty<object>()));

        #endregion Properties

        static ToolBarContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBarContainer), new FrameworkPropertyMetadata(typeof(ToolBarContainer)));
        }

        public Style ToolBarStyle
        {
            get { return (Style)GetValue(ToolBarStyleProperty); }
            set { SetValue(ToolBarStyleProperty, value); }
        }

        public IEnumerable<object> ToolBarItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ToolBarItemsSourceProperty); }
            set { SetValue(ToolBarItemsSourceProperty, value); }
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.Property == ToolBarStyleProperty)
            {
                if(ToolBarStyle != null && ToolBarStyle.TargetType == typeof(ToolBar))
                {
                    foreach (var toolBar in ToolBars)
                    {
                        toolBar.Style = Style;
                        toolBar.ApplyTemplate();
                    }
                }
            }
            
            else if(e.Property == ToolBarItemsSourceProperty)
            {
                ToolBars.Clear();
                foreach(var toolBar in CreateToolBars())
                {
                    ToolBars.Add(toolBar);
                }
            }

            base.OnPropertyChanged(e);
        }

        private IEnumerable<ToolBar> CreateToolBars()
        {
            foreach(var item in ToolBarItemsSource)
            {
                var toolBar = new ToolBar();
                if(ToolBarStyle != null && ToolBarStyle.TargetType == typeof(ToolBar))
                {
                    toolBar.Style = ToolBarStyle;
                    toolBar.ApplyTemplate();
                }
                toolBar.DataContext = item;

                yield return toolBar;
            }
        }

    }
}

using System.Windows;
using System.Windows.Controls;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Interaction logic for MainMenuView.xaml
    /// </summary>
    public partial class MainMenuView : UserControl, IMainMenuView
    {
        public MainMenuView()
        {
            InitializeComponent();
        }
    }

    public class MainMenuItemContainerTemplateSelector : ItemContainerTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl)
        {
            var key = new DataTemplateKey(item.GetType());
            return (DataTemplate)parentItemsControl.FindResource(key);
        }
    }
}

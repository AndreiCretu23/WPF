using Quantum.UIComponents;
using Quantum.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WPF.Panels;
using Quantum.Common;
using Microsoft.Practices.Composite.Presentation.Commands;

namespace WPF.ToolBars
{
    [Guid("ACC63832-E337-4B27-821A-DDEEA52FD95E")]
    public interface IFirstToolBarViewModel
    {
        DelegateCommand<object> AddPanelCommand { get; }
    }

    [Guid("10EE9E8C-8C29-4FAF-AFA7-B80BC63E8156")]
    public class FirstToolBarViewModel : ViewModelBase, IFirstToolBarViewModel
    {
        [Selection]
        public DynamicPanelSelection SelectedPanels { get; set; }
        
        public FirstToolBarViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }


        public DelegateCommand<object> AddPanelCommand => new DelegateCommand<object>
            (
                canExecuteMethod: o => true, 
                executeMethod: o => AddPanel()
            );


        private void AddPanel()
        {
            var panelId = GetFirstFreeNumber(SelectedPanels.Value.OfType<IIdentifiable>().Select(o => Int32.Parse(o.Guid)));
            SelectedPanels.Value.Add(new DynamicPanelViewModel(InitializationService)
            {
                Guid = panelId.ToString(),
                DisplayText = $"DynamicPanel {panelId.ToString()}"
            });
        }


        private int GetFirstFreeNumber(IEnumerable<int> values)
        {
            int i = 1;
            while (true)
            {
                if(!values.Contains(i))
                {
                    return i;
                }

                i++;
            }
        }

    }
}

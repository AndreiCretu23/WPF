﻿using Microsoft.Practices.Composite.Presentation.Commands;
using Quantum.Services;
using Quantum.UIComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WPF.Panels;

namespace WPF.ToolBars
{
    [Guid("E28C24EF-77A5-44CE-B446-120C940BA323")]
    public interface ISecondToolBarViewModel
    {
    }

    [Guid("C63CB9E1-8540-4D8D-85EE-DA38D28DAA5F")]
    public class SecondToolBarViewModel : ViewModelBase, ISecondToolBarViewModel
    {
        [Service]
        public IPanelManagerService PanelManager { get; set; }

        [Selection]
        public DynamicPanelSelection PanelSelection { get; set; }

        public SecondToolBarViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        #region StaticPanelBringIntoView

        public DelegateCommand<object> ShowPanelCommand => new DelegateCommand<object>
            (
                canExecuteMethod: o => true, 
                executeMethod: o => PanelManager.BringStaticPanelIntoView<IActivePanelViewModel>()
            );

        #endregion StaticPanelBringIntoView

        public DelegateCommand<object> ShowDynamicPanelCommand => new DelegateCommand<object>
            (
                canExecuteMethod: o => PanelSelection.Value.Count() > 0,
                executeMethod: o =>
                {
                    var viewModel = PanelSelection.Value.First();
                    PanelManager.BringDynamicPanelIntoView(viewModel);
                }
            );
    }
}

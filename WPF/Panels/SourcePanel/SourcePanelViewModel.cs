﻿using Quantum.Command;
using Quantum.Services;
using Quantum.UIComponents;
using Quantum.UIComposition;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace WPF.Panels
{
    [Guid("00D8BA2E-ED54-45A6-8098-0FC992C35627")]
    public class SourcePanelViewModel : ViewModelBase, ISourcePanelViewModel
    {
        public SourcePanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
    }
}

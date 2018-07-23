using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Quantum.Events;

namespace Quantum.Services
{
    internal class WPFEventManagerService : QuantumServiceBase, IWPFEventManagerService
    {
        public WPFEventManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public void HookWpfEvents()
        {
            HookApplicationExitEvent();
            HookUnhandledExceptionEvent();
        }

        private void HookApplicationExitEvent()
        {
            Application.Current.Exit += (sender, e) => EventAggregator.GetEvent<ApplicationExitEvent>().Publish(new ApplicationExitArgs());
        }

        private void HookUnhandledExceptionEvent()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                EventAggregator.GetEvent<UnhandledExceptionEvent>().Publish(new UnhandledExceptionArgs());
            };
        }
    }
}

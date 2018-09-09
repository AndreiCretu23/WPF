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
    internal class WPFEventManagerService : ServiceBase, IWPFEventManagerService
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
            Application.Current.Exit += (sender, e) => EventAggregator.GetEvent<ShutdownEvent>().Publish(new ShutdownArgs());
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

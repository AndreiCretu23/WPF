using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Command;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.UIComponents
{
    internal class MainMenuViewModel : ViewModelBase, IMainMenuViewModel
    {
        private IMainMenuCommandExtractor CommandExtractor { get; }

        public MainMenuViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            CommandExtractor = new MainMenuCommandExtractor(initSvc);
        }

        public IEnumerable<IMainMenuItemViewModel> Children { get { return CreateMenuContent(); } }

        private IEnumerable<IMainMenuItemViewModel> CreateMenuContent()
        {
            var rootPaths = CommandExtractor.AbstractMenuPaths.Where(path => path.ParentPath == AbstractMenuPath.Root).OrderBy(path => path.OrderIndex);
            foreach(var rootPath in rootPaths)
            {
                yield return new MainMenuPathViewModel(InitializationService, CommandExtractor, rootPath);
            }
        }
        
    }
}

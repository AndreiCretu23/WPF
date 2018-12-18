using Quantum.Services;
using Quantum.UIComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace WPF.Panels
{
    [Guid("00D8BA2E-ED54-45A6-8098-0FC992C35627")]
    public class SourcePanelViewModel : ViewModelBase, ISourcePanelViewModel
    {
        public IEnumerable<ViewModelItem> Children { get { return CreateChildren(); } }

        public SourcePanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        private IEnumerable<ViewModelItem> CreateChildren()
        {
            for(int i = 0; i < 10; i++) {
                yield return new ViewModelItem();
            }

            yield return new ViewModelItem()
            {
                Children = new ObservableCollection<ViewModelItem>()
                {
                    new ViewModelItem(),
                    new ViewModelItem(),
                    new ViewModelItem()
                    {
                        Children = new ObservableCollection<ViewModelItem>()
                        {
                            new ViewModelItem(),
                            new ViewModelItem(),
                            new ViewModelItem(),
                            new ViewModelItem(),
                        }
                    },
                    new ViewModelItem(),
                    new ViewModelItem(),
                }
            };

            for (int i = 0; i < 450; i++) {
                yield return new ViewModelItem();
            }
        }

    }

    public class ViewModelItem
    {
        public ObservableCollection<ViewModelItem> Children { get; set; } = new ObservableCollection<ViewModelItem>();
    }
}

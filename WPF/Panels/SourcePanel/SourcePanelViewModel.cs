using Quantum.Services;
using Quantum.UIComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private IEnumerable<ViewModelItem> GetChildrenInternal()
        {
            yield return new ViewModelItem();
            yield return new ViewModelItem();
            yield return new ViewModelItem()
            {
                ChildrenGetter = GetChildrenInternal
            };
            yield return new ViewModelItem();
            yield return new ViewModelItem();
        }

        private IEnumerable<ViewModelItem> CreateChildren()
        {
            for(int i = 0; i < 10; i++) {
                yield return new ViewModelItem();
            }

            yield return new ViewModelItem()
            {
                ChildrenGetter = GetChildrenInternal
            };

            for (int i = 0; i < 450; i++) {
                yield return new ViewModelItem();
            }
        }

    }

    public class ViewModelItem
    {
        public static int InstanceCount = 0;

        public ViewModelItem()
        {
            InstanceCount++;
        }

        public Func<IEnumerable<ViewModelItem>> ChildrenGetter { get; set; }

        private IEnumerable<ViewModelItem> CreateChildren()
        {
            if(ChildrenGetter != null) {
                return ChildrenGetter();
            }

            else {
                return Enumerable.Empty<ViewModelItem>();
            }
        }

        public IEnumerable<ViewModelItem> Children { get { return CreateChildren(); } }
    }
}

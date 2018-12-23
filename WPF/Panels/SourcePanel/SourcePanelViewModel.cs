using Quantum.Services;
using Quantum.UIComponents;
using Quantum.UIComposition;
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

        private IEnumerable<ViewModelItem> GetChildrenInternal(string head)
        {
            yield return new ViewModelItem() { Header = head + "1" };
            yield return new ViewModelItem() { Header = head + "2" , ChildrenGetter = GetChildrenInternal };
            yield return new ViewModelItem() { Header = head + "2" };
            yield return new ViewModelItem() { Header = head + "3" , ChildrenGetter = GetChildrenInternal };
            yield return new ViewModelItem() { Header = head + "4" };
        }

        private IEnumerable<ViewModelItem> CreateChildren()
        {
            for(int i = 0; i < 500; i++) {
                yield return new ViewModelItem()
                {
                    Header = i.ToString(),
                    ChildrenGetter = GetFunc(i),
                };
            }

        }

        private Func<string, IEnumerable<ViewModelItem>> GetFunc(int number)
        {
            if(number % 5 == 0) {
                return GetChildrenInternal;
            }

            return null;
        }

    }

    public class ViewModelItem : ObservableObject
    {
        public static int InstanceCount = 0;

        private string header;
        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                RaisePropertyChanged(() => Header);
            }
        }

        public ViewModelItem()
        {
            InstanceCount++;
        }

        public Func<string, IEnumerable<ViewModelItem>> ChildrenGetter { get; set; }

        private IEnumerable<ViewModelItem> CreateChildren()
        {
            if(ChildrenGetter != null) {
                return ChildrenGetter(header);
            }

            else {
                return Enumerable.Empty<ViewModelItem>();
            }
        }

        public IEnumerable<ViewModelItem> Children { get { return CreateChildren(); } }
    }
}

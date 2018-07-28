using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Quantum.UIComponents
{
    internal class ToolBarViewModel : ViewModelBase
    {
        private int band;
        public int Band
        {
            get { return band; }
            set
            {
                band = value;
                RaisePropertyChanged(() => Band);
                RaiseLayoutChanged();
            }
        }

        private int bandIndex;
        public int BandIndex
        {
            get { return bandIndex; }
            set
            {
                bandIndex = value;
                RaisePropertyChanged(() => BandIndex);
                RaiseLayoutChanged();
            }
        }

        public bool IsVisible { get { return VisibilityDelegate(); } }

        private object content;
        public object Content
        {
            get { return content; }
            set
            {
                content = value;
                RaisePropertyChanged(() => Children);
            }
        }

        public IEnumerable<object> Children
        {
            get
            {
                var viewModel = new ToolBarContentViewModel()
                {
                    Content = Content
                };
                return viewModel.ToEnumerable();
            }
        }

        public ToolBarViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        private Func<bool> visibilityDelegate;
        public Func<bool> VisibilityDelegate
        {
            get
            {
                return visibilityDelegate ?? (() => true);
            }
            set
            {
                visibilityDelegate = value;
                RaiseVisibilityChanged();
            }
        }

        private void RaiseLayoutChanged()
        {
            if (Content == null) return;

            EventAggregator.GetEvent<ToolBarLayoutChangedEvent>().Publish(new ToolBarLayoutChangedArgs()
            {
                View = Content.GetType(), 
                ViewModel = ((UserControl)Content).DataContext.GetType(), 
                Band = Band, 
                BandIndex = BandIndex,
            });
        }

        public void RaiseVisibilityChanged()
        {
            RaisePropertyChanged(() => IsVisible);
        }
    }
}

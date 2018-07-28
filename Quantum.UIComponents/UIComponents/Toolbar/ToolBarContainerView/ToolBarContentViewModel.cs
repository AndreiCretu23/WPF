using Quantum.UIComposition;

namespace Quantum.UIComponents
{
    internal class ToolBarContentViewModel : ObservableObject
    {
        private object content;
        public object Content
        {
            get { return content; }
            set
            {
                content = value;
                RaisePropertyChanged(() => Content);
            }
        }
    }
}

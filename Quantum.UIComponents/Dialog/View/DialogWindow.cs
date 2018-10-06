using Quantum.Controls;
using Quantum.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Quantum.UIComponents
{
    public class DialogWindow : WindowShell, IDialogWindow
    {
        #region DependencyProperties

        public static readonly DependencyProperty AbortsOnEscapeProperty =
            DependencyProperty.Register("AbortsOnEscape", typeof(bool), typeof(DialogWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty ValidatesOnEnterProperty =
            DependencyProperty.Register("ValidatesOnEnter", typeof(bool), typeof(DialogWindow), new PropertyMetadata(false));
        
        #endregion DependencyProperties

        #region Properties

        public bool AbortsOnEscape
        {
            get { return (bool)GetValue(AbortsOnEscapeProperty); }
            set { SetValue(AbortsOnEscapeProperty, value); }
        }

        public bool ValidatesOnEnter
        {
            get { return (bool)GetValue(ValidatesOnEnterProperty); }
            set { SetValue(ValidatesOnEnterProperty, value); }
        }
        
        #endregion Properties

        static DialogWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogWindow), new FrameworkPropertyMetadata(typeof(WindowShell)));
        }
        
        public DialogWindow()
        {
            DataContextChanged += OnDataContextChanged;
        }

        #region ViewModel

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is DialogViewModel)
            {
                var newConext = (DialogViewModel)e.NewValue;
                newConext.OnCloseRequest += result =>
                {
                    DialogResult = result;
                    DataContext = null;
                    Close();
                };
            }
        }

        #endregion ViewModel

        #region Shortcuts

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if(e.Key == Key.Escape && AbortsOnEscape)
            {
                if(DataContext != null && DataContext is DialogViewModel viewModel)
                {
                    if(viewModel.CanAbort())
                    {
                        viewModel.Abort();
                        DialogResult = false;
                        Close();
                    }
                }
                else
                {
                    DialogResult = false;
                    Close();
                }
            }

            else if(e.Key == Key.Enter && ValidatesOnEnter)
            {
                if(DataContext != null && DataContext is DialogViewModel viewModel)
                {
                    if(viewModel.ValidateContent())
                    {
                        viewModel.SaveChanges();
                        DialogResult = true;
                        Close();
                    }
                }
                else
                {
                    DialogResult = true;
                    Close();
                }
            }

            base.OnKeyDown(e);
        }

        #endregion Shortcuts

        #region CloseButton

        protected override void InitializeCloseButton(Button closeButton)
        {
            if(DataContext != null && (DataContext is IDialogViewModel viewModel))
            {
                BindingOperations.ClearBinding(closeButton, Button.CommandProperty);

                closeButton.SetBinding(Button.CommandProperty, new Binding()
                {
                    Path = new PropertyPath(ReflectionUtils.GetPropertyInfo((IDialogViewModel vm) => vm.AbortCommand)), 
                    Source = viewModel
                });
            }
            else
            {
                base.InitializeCloseButton(closeButton);
            }
        }

        #endregion CloseButton
    }
}

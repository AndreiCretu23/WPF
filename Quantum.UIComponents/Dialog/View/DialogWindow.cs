using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Quantum.UIComponents
{
    public class DialogWindow : Window, IDialogWindow
    {
        #region DependencyProperties

        public static readonly DependencyProperty AbortsOnEscapeProperty =
            DependencyProperty.Register("AbortsOnEscape", typeof(bool), typeof(DialogWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty ValidatesOnEnterProperty =
            DependencyProperty.Register("ValidatesOnEnter", typeof(bool), typeof(DialogWindow), new PropertyMetadata(false));

        #endregion DependencyProperties
        
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
        
        public DialogWindow()
        {
            DataContextChanged += OnDataContextChanged;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (DataContext != null && DataContext is DialogViewModel viewModel)
            {
                if (e.Key == Key.Escape && AbortsOnEscape && viewModel.CanAbort())
                {
                    viewModel.Abort();
                    DialogResult = false;
                    Close();
                }

                else if (e.Key == Key.Enter && ValidatesOnEnter && viewModel.ValidateContent())
                {
                    viewModel.SaveChanges();
                    DialogResult = true;
                    Close();
                }
            }
            base.OnKeyDown(e);
        }
        
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != null && e.NewValue is DialogViewModel)
            {
                var newConext = (DialogViewModel)e.NewValue;
                newConext.OnCloseRequest += result =>
                {
                    DialogResult = result;
                    Close();
                };
            }
        }
    }
}

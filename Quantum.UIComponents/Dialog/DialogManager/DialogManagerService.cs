using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Quantum.UIComponents
{
    internal class DialogManagerService : ServiceBase, IDialogManagerService
    {
        private Collection<IDialogDefinition> registeredDefinitions = new Collection<IDialogDefinition>();
        public IEnumerable<IDialogDefinition> RegisteredDefinitions { get { return registeredDefinitions; } }

        public DialogManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        #region Register

        public void RegisterAllDefinitions(IEnumerable<IDialogDefinition> definitions)
        {
            definitions.AssertNotNull(nameof(definitions));
            foreach(var definition in definitions)
            {
                RegisterDialogDefinition(definition);
            }
        }

        public void RegisterDialogDefinition(IDialogDefinition definition)
        {
            AssertDialogDefinition(definition);
            if(RegisteredDefinitions.Any(o => o.View == definition.View || 
                                              o.IView == definition.IView ||
                                              o.ViewModel == definition.ViewModel ||
                                              o.IViewModel == definition.IViewModel))
            {
                throw new Exception($"Error : DialogDefinition<{definition.IView.Name}, {definition.View.Name}, {definition.IViewModel.Name}, {definition.ViewModel.Name}> \n " +
                                    $"has already been registered or the View/IView/ViewModel/IViewModel have already been registered in another definition");
            }

            if (definition.SingleViewModelInstance)
            {
                Container.RegisterService(definition.IViewModel, definition.ViewModel);
            }
            else
            {
                Container.RegisterType(definition.IViewModel, definition.ViewModel);
            }

            registeredDefinitions.Add(definition);
        }

        #endregion Register

        #region Assert

        [DebuggerHidden]
        private void AssertDialogDefinition(IDialogDefinition definition)
        {
            definition.AssertNotNull(nameof(definition));
            definition.AssertNotNull(nameof(definition.View));
            definition.AssertNotNull(nameof(definition.IView));
            definition.AssertNotNull(nameof(definition.ViewModel));
            definition.AssertNotNull(nameof(definition.IViewModel));

            if(!definition.IView.IsInterface || !typeof(IDialogWindow).IsAssignableFrom(definition.IView))
            {
                throw new Exception($"Error : DialogDefinition<{definition.IView.Name}, {definition.View.Name}, {definition.IViewModel.Name}, {definition.ViewModel.Name}> \n" +
                                    $"The IView of a DialogDefinition must be an interface that extends {typeof(IDialogWindow).Name}. \n" +
                                    $"{definition.IView.Name} does not meet this criteria.");
            }

            if (!definition.View.IsClass ||
                 definition.View.GetConstructors().Count() != 1 || 
                 definition.View.GetConstructors().Single().GetParameters().Count() != 0 ||
                !definition.View.IsSubclassOf(typeof(DialogWindow)) || 
                !definition.View.Implements(definition.IView))
            {
                throw new Exception($"Error : DialogDefinition<{definition.IView.Name}, {definition.View.Name}, {definition.IViewModel.Name}, {definition.ViewModel.Name}> \n" +
                                    $"The View of a DialogDefinition must be a class that extends {typeof(DialogWindow).Name} and implements {definition.IView.Name}. \n " +
                                    $"with a single constructor that takes no parameters. \n" +
                                    $"{definition.View.Name} does not meet this criteria.");
            }

            if(!definition.IViewModel.IsInterface || !typeof(IDialogViewModel).IsAssignableFrom(definition.IViewModel))
            {
                throw new Exception($"Error : DialogDefinition<{definition.IView.Name}, {definition.View.Name}, {definition.IViewModel.Name}, {definition.ViewModel.Name}> \n" +
                                    $"The IViewModel of a DialogDefinition must be an interface that extends {typeof(IDialogViewModel).Name}. \n" +
                                    $"{definition.IViewModel.Name} does not meet this criteria.");
            }

            if(!definition.ViewModel.IsClass || 
               !definition.ViewModel.IsSubclassOf(typeof(DialogViewModel)) ||
               !definition.ViewModel.Implements(definition.IViewModel))
            {
                throw new Exception($"Error : DialogDefinition<{definition.IView.Name}, {definition.View.Name}, {definition.IViewModel.Name}, {definition.ViewModel.Name}> \n" +
                                    $"The ViewModel of a DialogDefinition must be a class that extends {typeof(DialogViewModel).Name} and implements {definition.IViewModel.Name}. \n" +
                                    $"{definition.ViewModel.Name} does not meet this criteria.");
            }
        }

        #endregion Assert

        #region ShowDialog

        private IDialogWindow CreateDialogView<TViewModel>() where TViewModel : IDialogViewModel
        {
            try
            {
                var definition = RegisteredDefinitions.Single(def => def.ViewModel == typeof(TViewModel) ||
                                                                     def.IViewModel == typeof(TViewModel));
                var viewType = definition.View;
                return (IDialogWindow)Activator.CreateInstance(viewType);
            }
            catch(InvalidOperationException)
            {
                throw;
            }
        }

        public bool? ShowDialog<TViewModel>() where TViewModel : IDialogViewModel
        {
            try
            {
                var view = CreateDialogView<TViewModel>();
                var viewModel = Container.Resolve<TViewModel>();
                view.DataContext = viewModel;
                return view.ShowDialog();
            }
            catch(InvalidOperationException)
            {
                throw new Exception($"Error : There is no registered dialog definition that matches the viewModel type {typeof(TViewModel).Name}");
            }
        }
        public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogViewModel
        {
            try
            {
                var view = CreateDialogView<TViewModel>();
                view.DataContext = viewModel;
                return view.ShowDialog();
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"Error : There is no registered dialog definition that matches the viewModel type {typeof(TViewModel).Name}");
            }
        }

        #endregion ShowDialog

        #region ShowMessageBox

        public MessageBoxResult ShowMessageBox(string messageBoxText)
        {
            throw new NotImplementedException();
        }
        public MessageBoxResult ShowMessageBox(string messageBoxText, string caption)
        {
            throw new NotImplementedException();
        }
        public MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button)
        {
            throw new NotImplementedException();
        }
        public MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            throw new NotImplementedException();
        }
        public MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            throw new NotImplementedException();
        }
        public MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        {
            throw new NotImplementedException();
        }

        #endregion ShowMessageBox
    }
}

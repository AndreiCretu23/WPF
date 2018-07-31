using Quantum.Events;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Quantum.UIComponents
{
    internal class ToolBarManagerService : QuantumServiceBase, IToolBarManagerService
    {
        private List<IToolBarDefinition> DefaultDefinitions { get; set; } = new List<IToolBarDefinition>();
        private List<IToolBarDefinition> ToolBarDefinitions { get; set; } = new List<IToolBarDefinition>();

        public ToolBarManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        

        #region Register

        public void RegisterToolBarDefinition<ITView, TView, ITViewModel, TViewModel>(ToolBarDefinition<ITView, TView, ITViewModel, TViewModel> toolbarDefinition)
            where TView : UserControl, ITView, new()
            where ITView : class
            where TViewModel : class, ITViewModel
            where ITViewModel : class
        {
            toolbarDefinition.AssertNotNull(nameof(toolbarDefinition));
            AssertTypes(typeof(ITView), typeof(TView), typeof(ITViewModel), typeof(TViewModel));
            ToolBarDefinitions.Add(toolbarDefinition);
            DefaultDefinitions.Add((IToolBarDefinition)toolbarDefinition.Clone());
            Container.RegisterService<ITViewModel, TViewModel>();
        }

        public void RegisterToolBarDefinition(IToolBarDefinition toolBarDefinition)
        {
            toolBarDefinition.AssertNotNull(nameof(toolBarDefinition));
            AssertTypes(toolBarDefinition.IView, toolBarDefinition.View, toolBarDefinition.IViewModel, toolBarDefinition.ViewModel);
            ToolBarDefinitions.Add(toolBarDefinition);
            DefaultDefinitions.Add((IToolBarDefinition)toolBarDefinition.Clone());
            Container.RegisterService(toolBarDefinition.IViewModel, toolBarDefinition.ViewModel);
        }

        #endregion Register

        #region Restore

        public void RestoreLayout()
        {
            EventAggregator.GetEvent<ToolBarLayoutRestoreRequest>().Publish(new ToolBarLayoutRestoreArgs(DefaultDefinitions));
        }

        #endregion Restore

        #region Extract

        public IEnumerable<IToolBarDefinition> GetToolBarDefinitions()
        {
            var customLayout = DeserializeToolBarLayout();
            
            if(customLayout != null && !ToolBarDefinitions.Any(def => !customLayout.Any(o => o.MatchesDefinition(def))))
            {
                foreach (var toolbarLayout in customLayout)
                {
                    var definition = ToolBarDefinitions.Single(def => toolbarLayout.MatchesDefinition(def));

                    definition.Band = toolbarLayout.Band;
                    definition.BandIndex = toolbarLayout.BandIndex;
                }
            }

            return ToolBarDefinitions;
        }

        #endregion Extract

        #region Assert

        [DebuggerHidden]
        private void AssertTypes(Type iView, Type view, Type iViewModel, Type viewModel)
        {
            iView.AssertParameterNotNull(nameof(iView));
            view.AssertParameterNotNull(nameof(view));
            iViewModel.AssertParameterNotNull(nameof(iViewModel));
            viewModel.AssertParameterNotNull(nameof(viewModel));
            
            view.AssertTypeHasGuid(ComposeAssertGuidMessage(view));
            iView.AssertTypeHasGuid(ComposeAssertGuidMessage(iView));
            viewModel.AssertTypeHasGuid(ComposeAssertGuidMessage(viewModel));
            iViewModel.AssertTypeHasGuid(ComposeAssertGuidMessage(iViewModel));

            
            if(!view.IsClass || 
               !view.IsSubclassOf(typeof(UserControl)) || 
               view.GetConstructors().Count() != 1 ||
               view.GetConstructors().Single().GetParameters().Any())
            {
                throw new Exception($"Error : {view.Name} must be a class inheriting from user control with an empty constructor.");
            }

            if(!view.Implements(iView))
            {
                throw new Exception($"Error : {view.Name} does not implement {iView.Name}.");
            }

            if(!viewModel.Implements(iViewModel))
            {
                throw new Exception($"Error : {viewModel.Name} does not implement {iViewModel.Name}.");
            }

            if(ToolBarDefinitions.Any(def => def.View == view)) {
                throw new Exception($"Error! View {view.Name} has already been registered.");
            }
            if (ToolBarDefinitions.Any(def => def.IView == iView)) {
                throw new Exception($"Error! View interface {iView.Name} has already been registered.");
            }
            if (ToolBarDefinitions.Any(def => def.ViewModel == viewModel)) {
                throw new Exception($"Error! ViewModel {viewModel.Name} has already been registered.");
            }
            if (ToolBarDefinitions.Any(def => def.IViewModel == iViewModel)) {
                throw new Exception($"Error! ViewModel interface {iViewModel.Name} has already been registered.");
            }
        }

        [DebuggerHidden]
        private string ComposeAssertGuidMessage(Type type)
        {
            return $"Error, {type.Name} does not have a Guid attribute. This is required for the serialization of the ToolBar Locations." + 
                   $"Please provide a unique guid via attribute.";
        }

        #endregion Assert

        #region Serialization

        private string ToolBarLayoutPath = Path.Combine(AppInfo.ApplicationConfigRepository, "ToolBarLayout.bin");

        private IEnumerable<ToolBarData> DeserializeToolBarLayout()
        {
            if(!File.Exists(ToolBarLayoutPath)) {
                return null;
            }

            return BinarySerializer.Deserialize<List<ToolBarData>>(ToolBarLayoutPath);
        }

        private void SerializeToolBarLayout()
        {
            var toolBarLayout = new List<ToolBarData>();
            foreach(var toolBarDefinition in ToolBarDefinitions)
            {
                toolBarLayout.Add(new ToolBarData()
                {
                    IViewGuid = toolBarDefinition.IView.GetGuid(), 
                    ViewGuid = toolBarDefinition.View.GetGuid(), 
                    IViewModelGuid = toolBarDefinition.IViewModel.GetGuid(), 
                    ViewModelGuid = toolBarDefinition.ViewModel.GetGuid(), 
                    Band = toolBarDefinition.Band, 
                    BandIndex = toolBarDefinition.BandIndex
                });
            }

            BinarySerializer.Serialize(toolBarLayout, ToolBarLayoutPath, true);
        }

        #endregion Serialization

        #region Events

        [Handles(typeof(ApplicationExitEvent))]
        public void OnApplicationExit()
        {
            SerializeToolBarLayout();
        }

        [Handles(typeof(ToolBarLayoutChangedEvent))]
        public void OnToolBarLayoutChanged(ToolBarLayoutChangedArgs args)
        {
            var definition = ToolBarDefinitions.Single(def => def.View == args.View &&
                                                              def.ViewModel == args.ViewModel);
            definition.Band = args.Band;
            definition.BandIndex = args.BandIndex;
        }

        #endregion Events
    }

    [Serializable]
    internal class ToolBarData
    {
        public string IViewGuid { get; set; }
        public string ViewGuid { get; set; }
        public string IViewModelGuid { get; set; }
        public string ViewModelGuid { get; set; }

        public int Band { get; set; }
        public int BandIndex { get; set; }

        public bool MatchesDefinition(IToolBarDefinition definition)
        {
            return IViewGuid == definition.IView.GetGuid()
                && ViewGuid == definition.View.GetGuid()
                && IViewModelGuid == definition.IViewModel.GetGuid()
                && ViewModelGuid == definition.ViewModel.GetGuid();
        }
    }
}

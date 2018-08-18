using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Common;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Quantum.UIComponents
{
    internal class PanelManagerService : QuantumServiceBase, IPanelManagerService
    {
        private List<IStaticPanelDefinition> staticPanelDefinitions = new List<IStaticPanelDefinition>();
        private List<IDynamicPanelDefinition> dynamicPanelDefinitions = new List<IDynamicPanelDefinition>();

        public IEnumerable<IStaticPanelDefinition> StaticPanelDefinitions { get { return staticPanelDefinitions; } }
        public IEnumerable<IDynamicPanelDefinition> DynamicPanelDefinitions { get { return dynamicPanelDefinitions; } }

        [Service]
        public IMetadataAsserterService MetadataAsserter { get; set; }

        public PanelManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        #region Register

        public void RegisterStaticPanelDefinition(IStaticPanelDefinition definition)
        {
            AssertStaticPanelDefinition(definition);
            Container.RegisterService(definition.IViewModel, definition.ViewModel);
            staticPanelDefinitions.Add(definition);
        }

        public void RegisterDynamicPanelDefinition(IDynamicPanelDefinition definition)
        {
            AssertDynamicPanelDefinition(definition);
            Container.RegisterType(definition.IViewModel, definition.ViewModel);
            dynamicPanelDefinitions.Add(definition);
        }

        public void RegisterPanelDefinition(IPanelDefinition definition)
        {
            definition.AssertParameterNotNull(nameof(definition));
            if(definition is IStaticPanelDefinition)
            {
                RegisterStaticPanelDefinition((IStaticPanelDefinition)definition);
            }
            else if(definition is IDynamicPanelDefinition)
            {
                RegisterDynamicPanelDefinition((IDynamicPanelDefinition)definition);
            }
            else
            {
                throw new Exception($"Error : You can only register a definition which either implements {typeof(IStaticPanelDefinition).Name}" +
                    $"or {typeof(IDynamicPanelDefinition).Name}");
            }
        }

        #endregion Register


        #region Assert

        [DebuggerHidden]
        private void AssertStaticPanelDefinition(IStaticPanelDefinition definition)
        {
            definition.AssertParameterNotNull(nameof(definition));

            definition.IView.AssertNotNull($"IStaticPanelDefinition.IView");
            definition.View.AssertNotNull($"IStaticPanelDefinition.View");
            definition.IViewModel.AssertNotNull($"IStaticPanelDefinition.IViewModel");
            definition.ViewModel.AssertNotNull($"IStaticPanelDefinition.ViewModel");

            definition.IView.AssertTypeHasGuid(ComposeAssertGuidMessage(definition.IView));
            definition.View.AssertTypeHasGuid(ComposeAssertGuidMessage(definition.View));
            definition.IViewModel.AssertTypeHasGuid(ComposeAssertGuidMessage(definition.IViewModel));
            definition.ViewModel.AssertTypeHasGuid(ComposeAssertGuidMessage(definition.ViewModel));


            if (!definition.View.IsClass ||
               !definition.View.IsSubclassOf(typeof(UserControl)) ||
               definition.View.GetConstructors().Count() != 1 ||
               definition.View.GetConstructors().Single().GetParameters().Any())
            {
                throw new Exception($"Error : {definition.View.Name} must be a class inheriting from user control with an empty constructor.");
            }

            if (!definition.View.Implements(definition.IView))
            {
                throw new Exception($"Error : {definition.View.Name} does not implement {definition.IView.Name}.");
            }

            if (!definition.ViewModel.Implements(definition.IViewModel))
            {
                throw new Exception($"Error : {definition.ViewModel.Name} does not implement {definition.IViewModel.Name}.");
            }

            if (StaticPanelDefinitions.Any(def => def.View == definition.View))
            {
                throw new Exception($"Error! A StaticPanelDefinition with the associated {definition.View.Name} View has already been registered.");
            }
            if (StaticPanelDefinitions.Any(def => def.IView == definition.IView))
            {
                throw new Exception($"Error! A StaticPanelDefinition with the associated View interface {definition.IView.Name} has already been registered.");
            }
            if (StaticPanelDefinitions.Any(def => def.ViewModel == definition.ViewModel))
            {
                throw new Exception($"Error! A StaticPanelDefinition with the associated ViewModel : {definition.ViewModel.Name} has already been registered.");
            }
            if (StaticPanelDefinitions.Any(def => def.IViewModel == definition.IViewModel))
            {
                throw new Exception($"Error! A StaticPanelDefinition with the associated ViewModel interface : {definition.IViewModel.Name} has already been registered.");
            }

            MetadataAsserter.AssertMetadataCollection<IStaticPanelDefinition, IStaticPanelMetadata>(definition, $"StaticPanelDefinition<{definition.IView.Name}, {definition.View.Name}, {definition.IViewModel.Name}, {definition.ViewModel.Name}>");
        }

        [DebuggerHidden]
        private void AssertDynamicPanelDefinition(IDynamicPanelDefinition definition)
        {
            definition.AssertParameterNotNull(nameof(definition));

            definition.IView.AssertNotNull($"IDynamicPanelDefinition.IView");
            definition.View.AssertNotNull($"IDynamicPanelDefinition.View");
            definition.IViewModel.AssertNotNull($"IDynamicPanelDefinition.IViewModel");
            definition.ViewModel.AssertNotNull($"IDynamicPanelDefinition.ViewModel");
            
            if (!definition.View.IsClass ||
               !definition.View.IsSubclassOf(typeof(UserControl)) ||
               definition.View.GetConstructors().Count() != 1 ||
               definition.View.GetConstructors().Single().GetParameters().Any())
            {
                throw new Exception($"Error : {definition.View.Name} must be a class inheriting from user control with an empty constructor.");
            }

            if (!definition.View.Implements(definition.IView))
            {
                throw new Exception($"Error : {definition.View.Name} does not implement {definition.IView.Name}.");
            }

            if (!definition.ViewModel.Implements(definition.IViewModel))
            {
                throw new Exception($"Error : {definition.ViewModel.Name} does not implement {definition.IViewModel.Name}.");
            }

            if(!definition.ViewModel.Implements(typeof(IIdentifiable)))
            {
                throw new Exception($"Error : {definition.ViewModel.Name} does not implement IIdentifiable. This is required for the serialization of the" +
                    $"active panel layout collection.");
            }

            if (DynamicPanelDefinitions.Any(def => def.View == definition.View))
            {
                throw new Exception($"Error! A DynamicPanelDefinitions with the associated {definition.View.Name} View has already been registered.");
            }
            if (DynamicPanelDefinitions.Any(def => def.IView == definition.IView))
            {
                throw new Exception($"Error! A DynamicPanelDefinitions with the associated View interface {definition.IView.Name} has already been registered.");
            }
            if (DynamicPanelDefinitions.Any(def => def.ViewModel == definition.ViewModel))
            {
                throw new Exception($"Error! A DynamicPanelDefinitions with the associated ViewModel : {definition.ViewModel.Name} has already been registered.");
            }
            if (DynamicPanelDefinitions.Any(def => def.IViewModel == definition.IViewModel))
            {
                throw new Exception($"Error! A DynamicPanelDefinitions with the associated ViewModel interface : {definition.IViewModel.Name} has already been registered.");
            }

            MetadataAsserter.AssertMetadataCollection<IDynamicPanelDefinition, IDynamicPanelMetadata>(definition, $"DynamicPanelDefinition<{definition.IView.Name}, {definition.View.Name}, {definition.IViewModel.Name}, {definition.ViewModel.Name}>");

            var config = definition.Single(o => o.GetType().IsGenericType && o.GetType().GetGenericTypeDefinition() == typeof(DynamicPanelConfiguration<>));
            var configGenericParamType = config.GetType().GetGenericArguments().Single();
            if(!(configGenericParamType == definition.View || 
                configGenericParamType == definition.IView ||
                configGenericParamType == definition.ViewModel || 
                configGenericParamType == definition.IViewModel))
            {
                throw new Exception($"Error : DynamicPanelDefinition<{definition.IView.Name}, {definition.View.Name}, {definition.IViewModel.Name}, {definition.ViewModel.Name}> : \n" +
                                    $"The DynamicPanelConfiguration Generic Parameter must be of one of the following types : {definition.IView.Name}, {definition.View.Name}, {definition.IViewModel.Name}, {definition.ViewModel.Name}");
            }

            var selectionBindingType = definition.GetSelectionBindingRawType();
            if(!(selectionBindingType == definition.ViewModel || selectionBindingType == definition.IViewModel))
            {
                throw new Exception($"Error : DynamicPanelDefinition<{definition.IView.Name}, {definition.View.Name}, {definition.IViewModel.Name}, {definition.ViewModel.Name}> : \n " +
                                    $"The PanelSelectionBinding type does not match the viewModel Type.");
            }

        }

        [DebuggerHidden]
        private string ComposeAssertGuidMessage(Type type)
        {
            return $"Error, {type.Name} does not have a Guid attribute. This is required for the serialization of the Panels' Layout." +
                   $"Please provide a unique guid via attribute.";
        }

        #endregion Assert


        #region Config

        private IDockingConfiguration DefaultDockingConfiguration = new DefaultDockingConfiguration();
        private IDockingConfiguration CustomDockingConfiguration;

        public IDockingConfiguration DockingConfiguration { get { return CustomDockingConfiguration ?? DefaultDockingConfiguration; } }

        public void SetDockingConfiguration(IDockingConfiguration configuration)
        {
            configuration.AssertParameterNotNull(nameof(configuration));
            if(configuration.SerializesLayout)
            {
                if(configuration.LayoutSerializationEvent == null)
                {
                    throw new Exception($"Error : LayoutSerializationEvent is null.");
                }
                if(configuration.LayoutDeserializationEvent == null)
                {
                    throw new Exception($"Error : LayoutDeserializationEvent is null.");
                }

                if(!configuration.LayoutSerializationEvent.IsSubclassOfRawGeneric(typeof(CompositePresentationEvent<>)))
                {
                    throw new Exception($"Error : {configuration.LayoutSerializationEvent.Name} is not a valid event type. The event type must be event-aggregator compatible, which means it must extend CompositePresentationEvent<TPayload>.");
                }
                if (!configuration.LayoutDeserializationEvent.IsSubclassOfRawGeneric(typeof(CompositePresentationEvent<>)))
                {
                    throw new Exception($"Error : {configuration.LayoutDeserializationEvent.Name} is not a valid event type. The event type must be event-aggregator compatible, which means it must extend CompositePresentationEvent<TPayload>.");
                }
            }

            CustomDockingConfiguration = configuration;
        }
        #endregion Config
    }
}

using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Quantum.UIComponents
{
    internal static class PanelDefinitionExt
    {
        
        #region StaticPanelDefinition

        internal static bool CanChangeVisibility(this IStaticPanelDefinition definition, bool currentVisibility)
        {
            definition.AssertNotNull(nameof(definition));

            var panelConfig = definition.OfType<StaticPanelConfiguration>().Single();
            return currentVisibility ? panelConfig.CanClose() : panelConfig.CanOpen();
        }

        #endregion StaticPanelDefinition

        #region DynamicPanelDefinition

        internal static IDynamicPanelMetadata GetConfig(this IDynamicPanelDefinition definition)
        {
            definition.AssertNotNull(nameof(definition));

            return definition.Single(o => o.GetType().IsGenericType &&
                                          o.GetType().GetGenericTypeDefinition() == typeof(DynamicPanelConfiguration<>));
        }

        internal static PanelPlacement GetPlacement(this IDynamicPanelDefinition definition)
        {
            definition.AssertNotNull(nameof(definition));

            var config = definition.GetConfig();
            return config.GetType().GetProperty("Placement").GetValue(config).SafeCast<PanelPlacement>();
        }

        private static object ComputeConfigDelegateResult(this IDynamicPanelDefinition definition, string delegatePropertyName, object view, object viewModel)
        {
            definition.AssertNotNull(nameof(definition));
            view.AssertParameterNotNull(nameof(view));
            viewModel.AssertParameterNotNull(nameof(viewModel));

            var config = definition.GetConfig();
            var configType = config.GetType().GetGenericArguments().Single();

            var del = config.GetType().GetProperty(delegatePropertyName).GetValue(config).SafeCast<Delegate>();

            if (configType == definition.View || configType == definition.IView)
            {
                return del.DynamicInvoke(view);
            }

            else if (configType == definition.ViewModel || configType == definition.IViewModel)
            {
                return del.DynamicInvoke(viewModel);
            }

            else
            {
                throw new Exception($"Internal Error : DynamicPanelDefinition<{definition.IView.Name}, {definition.View.Name}, {definition.IViewModel.Name}, {definition.ViewModel.Name}> : \n " +
                    $"DynamicPanelConfiguration Generic Type is not valid and has not been correctly asserted.");
            }
        }

        internal static string ComputeTitle(this IDynamicPanelDefinition definition, object view, object viewModel)
        {
            return definition.ComputeConfigDelegateResult("Title", view, viewModel).SafeCast<string>();
        }

        internal static bool ComputeCanFloat(this IDynamicPanelDefinition definition, object view, object viewModel)
        {
            return definition.ComputeConfigDelegateResult("CanFloat", view, viewModel).SafeCast<bool>();
        }
        
        internal static Type GetSelectionBindingType(this IDynamicPanelDefinition definition)
        {
            definition.AssertNotNull(nameof(definition));
            return definition.OfType<PanelSelectionBinding>().Single().SelectionType;
        }

        internal static Type GetSelectionBindingRawType(this IDynamicPanelDefinition definition)
        {
            definition.AssertNotNull(nameof(definition));
            return definition.GetSelectionBindingType().GetBaseTypeGenericArgument(typeof(MultipleSelection<>));
        }

        internal static IMultipleSelection GetSelectionBinding(this IDynamicPanelDefinition definition, IUnityContainer container)
        {
            definition.AssertNotNull(nameof(definition));
            definition.AssertParameterNotNull(nameof(container));

            var selectionType = definition.GetSelectionBindingType();
            var eventAggregator = container.Resolve<IEventAggregator>();

            return eventAggregator.GetEvent(selectionType).SafeCast<IMultipleSelection>();
        }

        #endregion DynamicPanelDefinition
    }
}

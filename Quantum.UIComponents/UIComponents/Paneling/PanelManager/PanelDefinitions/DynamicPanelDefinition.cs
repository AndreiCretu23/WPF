using Quantum.Common;
using Quantum.Metadata;
using System;
using System.Windows.Controls;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Dynamic panel definitions represent a set of metadata that, if registered in the panel manager service 
    /// of the framework, will be processed by the framework in the form of multiple panel entries of the same 
    /// view inside the docking manager. The list of active panels is managed by multiple selection binding 
    /// of the viewModel type associated with this definition. The appearence and behavior of the panel entries
    /// is defined by the dynamic panel definition's own metadata : <para/>
    /// 1) DynamicPanelConfiguration - Represents a set of metadata definitions defining the panel entries' 
    ///                                initial position and basic behavior (Check DynamicPanelConfiguration for
    ///                                details). The generic parameter represents the instance of a panel entry
    ///                                (can be any of the ITView, TView, ITViewModel, TViewModel types associated 
    ///                                to this DynamicPanelDefinition), from which different configuration options 
    ///                                for it can be deduced by setting different options inside the instance. 
    ///                                This metadata type is mandatory inside a DynamicPanelDefinition. <para/>
    /// 2) PanelSelectionBinding - Represents the type of a multiple selection of the view model type associated with
    ///                            this dynamic panel definition that determins the current selection of active 
    ///                            panel entries. The multiple selection type specified will be resolved by the 
    ///                            framework from the event aggregator instance of the application's container.
    ///                            When a panel entry is closed, the instance of it's associated view model will 
    ///                            be removed from the selection by the framework. When a new view model instance 
    ///                            is added in the selection, the framework will automatically create a new panel 
    ///                            entry for the new viewModel instance. Also, if a viewModel instance is 
    ///                            manually removed from the selection, the framework will close the panel 
    ///                            associated with the removed viewModel instance. 
    ///                            This metadata type is mandatory inside a DynamicPanelDefinition. <para/>
    /// 3) AutoInvalidateOnEvent - Instructs the framework to invalidate the configuration associated with this 
    ///                            DynamicPanelDefinition for each active panel when the event of the specified type 
    ///                            is raised in the event aggregator instance of the application's container. <para/>
    /// 4) AutoInvalidateOnSelection - Instructs the framework to invalidate the configuration associated with this 
    ///                                DynamicPanelDefinition for each active panel when the selection of the specified
    ///                                type resolved from the event aggregator instance of the application's container
    ///                                changes.
    ///                            
    /// </summary>
    /// <typeparam name="ITView">The type of the interface the view implements.</typeparam>
    /// <typeparam name="TView">The type of the view associated to this DynamicPanelDefinition.</typeparam>
    /// <typeparam name="ITViewModel">The type of the interface the viewModel implements.</typeparam>
    /// <typeparam name="TViewModel">The type of the viewModel associated to the view of this DynamicPanelDefinition.</typeparam>
    public class DynamicPanelDefinition<ITView, TView, ITViewModel, TViewModel> : MetadataCollection<IDynamicPanelMetadata>, IDynamicPanelDefinition
        where TView : UserControl, ITView, new()
        where ITView : class
        where TViewModel : class, ITViewModel, IIdentifiable
        where ITViewModel : class
    {
        /// <summary>
        /// Returns the type of the interface the view implements.
        /// </summary>
        public Type IView => typeof(ITView);

        /// <summary>
        /// Returns the type of the view associated to this DynamicPanelDefinition.
        /// </summary>
        public Type View => typeof(TView);

        /// <summary>
        /// Returns the type of the interface the viewModel implements.
        /// </summary>
        public Type IViewModel => typeof(ITViewModel);

        /// <summary>
        /// Returns the type of the viewModel associated to the view of this DynamicPanelDefinition.
        /// </summary>
        public Type ViewModel => typeof(TViewModel);
    }
}

using Quantum.Metadata;
using System;
using System.Windows.Controls;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Static panel definitions represent a set of metadata that, if registered in the panel manager service
    /// of the framework, will be processed by the framework in the form of a single panel view inside the 
    /// docking manager that can be opened/closed. The appearence and behavior of the panel is defined by the 
    /// static panel definition's own metadata : <para/>
    /// 1) StaticPanelConfiguration -> Represents a set of metadata definitions defining the panel's initial position 
    ///                                basic behavior (Check StaticPanelConfiguration for details). 
    ///                                This metadata type is mandatory in a StaticPanelDefinition.<para/>
    /// 2) PanelMenuOption -> The panel will have an associated entry in the main menu of the application(menu location
    ///                       and other factors are customizable by setting different metadata types in the 
    ///                       PanelMenuOption's own metadata). The main menu entry will have an associated command 
    ///                       that will bring the panel into view and it's state will be dependant on the 
    ///                       panel's current state and configuration. This metadata type does not support multiple
    ///                       instances in the parent StaticPanelDefinition. <para/>
    /// 3) AutoInvalidateOnEvent -> This metadata type instructs the framework to re-evaluate the panel's 
    ///                             state / configuration when the event of the specified type is
    ///                             fired in the event aggregator instance of the application's container. <para/>
    /// 4) AutoInvalidateOnSelection -> This metadata type instructs the framework to re-evaluate the panel's
    ///                                 state / configuration when the selection of the specified type 
    ///                                 resolved from the event aggregator instance of the application's container
    ///                                 changes. <para/>
    /// 5) BringIntoViewOnKeyShortcut -> The panel will be brought into view when the specified key combination is pressed.
    ///                                  This will occur only if the panel CanOpen at the requested time, 
    ///                                  determined by the delegate in the static panel configuration.
    ///                                  This metadata type does not support multiple instances in the parent StaticPanelDefinition. <para/>
    /// 6) BringIntoViewOnEvent -> The panel will be brought into view when the event of the specified type is published. <para/>
    /// 7) BringIntoViewOnSelection -> The panel will be brought into view when the selection of the specified type changes. <para/>
    /// 
    /// </summary>
    /// <typeparam name="ITView">The type of the interface the view implements.</typeparam>
    /// <typeparam name="TView">The type of the view associated to this StaticPanelDefinition.</typeparam>
    /// <typeparam name="ITViewModel">The type of the interface the viewModel implements.</typeparam>
    /// <typeparam name="TViewModel">The type of the viewModel associated to the view of this StaticPanelDefinition.</typeparam>
    public class StaticPanelDefinition<ITView, TView, ITViewModel, TViewModel> : MetadataCollection<IStaticPanelMetadata>, IStaticPanelDefinition
        where TView : UserControl, ITView, new()
        where ITView : class
        where TViewModel : class, ITViewModel
        where ITViewModel : class
    {
        /// <summary>
        /// Returns the type of the interface the view implements.
        /// </summary>
        public Type IView => typeof(ITView);

        /// <summary>
        /// Returns the type of the view associated to this StaticPanelDefinition.
        /// </summary>
        public Type View => typeof(TView);

        /// <summary>
        /// Returns the type of the interface the viewModel implements.
        /// </summary>
        public Type IViewModel => typeof(ITViewModel);

        /// <summary>
        /// Returns the type of the viewModel associated to the view of this StaticPanelDefinition.
        /// </summary>
        public Type ViewModel => typeof(TViewModel);
    }
}

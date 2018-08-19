using System.Collections.Generic;

namespace Quantum.UIComponents
{
    public interface IPanelManagerService
    {
        /// <summary>
        /// Retrieves the Static Panel Definitions that are currently registered.
        /// </summary>
        IEnumerable<IStaticPanelDefinition> StaticPanelDefinitions { get; }

        /// <summary>
        /// Retrieves the Dynamic Panel Definitions that are currently registered.
        /// </summary>
        IEnumerable<IDynamicPanelDefinition> DynamicPanelDefinitions { get; }

        /// <summary>
        /// Returns the docking configuration used by the PanelManager.
        /// </summary>
        IDockingConfiguration DockingConfiguration { get; }

        /// <summary>
        /// Registers the given IStaticPanelDefinition instance. It is recommended that you use the 
        /// StaticPanelDefinition <![CDATA[]]>TIView, TView, TIViewModel, TViewModel> class and not create your own class that implemetnts IStaticPanelDefinition.
        /// A StaticPanelDefinition is a View/ViewModel Pair that only gets registered once, meaning it's a single panel view, such as the Visual Studio Solution Explorer : 
        /// Multiple instances of the view are not supported.
        /// </summary>
        /// <param name="definition"></param>
        void RegisterStaticPanelDefinition(IStaticPanelDefinition definition);

        /// <summary>
        /// Registers the given IDynamicPanelDefinition instance. It is recommended that you use the 
        /// DynamicPanelDefinition  <![CDATA[]]>TIView, TView, TIViewModel, TViewModel> and not create your own class that implemetnts IDynamicPanelDefinition.
        /// A DynamicPanelDefinition is a View/ViewModel Pair that only gets registered by type, suports multiple view instances, 
        /// and through a SelectionBinding(which must be passed in as a metadata), the currently active view(actual viewModel) panel collection is hold.
        /// An example of what this is is the Visual Studio Text Editor Panel.
        /// </summary>
        /// <param name="definition"></param>
        void RegisterDynamicPanelDefinition(IDynamicPanelDefinition definition);

        /// <summary>
        /// Registers the given IPanelDefinition instance. It's evaluated whether passed definition is an IStaticPanelDefinition or an IDynamicPanelDefinition, 
        /// and then registered accordingly. Other types are not supported.
        /// </summary>
        /// <param name="definition"></param>
        void RegisterPanelDefinition(IPanelDefinition definition);

        /// <summary>
        /// Brings the static panel associated with the given ViewModel type into view. If the panel is active, the parent container will have it selected. 
        /// Otherwise, if it's hidden, it the panel will become visible and selected in it's cached container.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        void BringStaticPanelIntoView<TViewModel>();

        /// <summary>
        /// Brings the dynamic panel instance of the given ViewModel type into view. The SelectionBinding associated with the DynamicPanelDefinition of the given type 
        /// must contain the instance passed as a parameter, otherwise an exception will be thrown.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewModel"></param>
        void BringDynamicPanelIntoView<TViewModel>(TViewModel viewModel);
        
        /// <summary>
        /// Sets the DockingConfiguration. If no docking configuration is provided, the default one will be used. In order to set
        /// a custom one, an instance of a custom implementation of IDockingConfiguration will have to be passed.
        /// </summary>
        /// <param name="configuration"></param>
        void SetDockingConfiguration(IDockingConfiguration configuration);
    }
}

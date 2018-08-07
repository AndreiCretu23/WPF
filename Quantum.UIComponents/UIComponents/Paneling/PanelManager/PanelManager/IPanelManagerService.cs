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
        /// Sets the DockingConfiguration. If no docking configuration is provided, the default one will be used. In order to set
        /// a custom one, an instance of a custom implementation of IDockingConfiguration will have to be passed.
        /// </summary>
        /// <param name="configuration"></param>
        void SetDockingConfiguration(IDockingConfiguration configuration);
    }
}

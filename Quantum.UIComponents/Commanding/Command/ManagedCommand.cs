using Quantum.Metadata;

namespace Quantum.Command
{
    /// <summary>
    /// Managed commands are application-wide parameterless commands processed by the MainWindow of the application. 
    /// They must be defined as properties in command containers registered in the command manager service of the framework.
    /// After they get registered in the command manager service cache, they are processed by the services which create the main window
    /// of the application depending on their metadata : <para/>
    /// 1) MainMenuOption - The command will appear as an entry in the main menu of the application(menu location and other factors are 
    ///                     customizable by setting different metadata types in the MainMenuOption's own metadata).
    ///                     This metadata type does not support multiple instances in the metadata collection of a managed command.<para/>
    /// 2) KeyShortcut - An application-wide shortcut for the command (The shortcut will trigger the command no matter which element of the MainWindow is focused).<para></para>
    /// 3) AutoInvalidateOnEvent - RaiseCanExecuteChanged() is automatically called when the event of the specified type is fired in the event aggregator
    ///                            instance of the application's container. Supported event types are types which extend CompositePresentationEvent. <para/>
    /// 4) AutoInvalidateOnSelection - RaiseCanExecuteChanged() is automatically called when the selection of the specified type (resolved from the 
    ///                                event aggregator instance of the application's container) changes. Supported selection types are types which extend SelectionBase.
    /// </summary>
    public class ManagedCommand : StaticMetadataOwnerCommand<CommandMetadataCollection, ICommandMetadata>, IManagedCommand
    {
    }
}

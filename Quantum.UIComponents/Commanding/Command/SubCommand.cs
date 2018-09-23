using Quantum.Metadata;

namespace Quantum.Command
{
    /// <summary>
    /// Subcommands are application-wide commads that are part of a set of commands return by a MultiManagedCommand's getter delegate which
    /// are processed by the main window of the application depending on the parent MultiManagedCommand's metadata and their own individual metadata.
    /// The parent MultiManagedCommand must be defined as a property in a command container registered in the command manager service of the framework.
    /// After the parent MultiManagedCommand gets registered and processed by the framework, the set of subcommands is computed and each of them is processed 
    /// by the services which create the main window of the application depending on their individual metadata : <para/>
    /// 1) SubMainMenuOption - The subcommand will appear as an option in the main menu of the application. The menu location depends 
    ///                        on the parent MultiManagedCommand's MultiMainMenuOption metadata (if the parent MultiManagedCommand does not have a 
    ///                        MultiMainMenuOption metadata defined, this metadata type does nothing) and the order in the set of commands returned by 
    ///                        the parent MultiManagedCommand's getter delegate. The other configuration options can be defined in the subcommand's 
    ///                        SubMainMenuOption (this) metada collection. <para/>
    /// 2) AutoInvalidateOnEvent - RaiseCanExecuteChanged() is automatically called when the event of the specified type is fired in the event aggregator
    ///                            instance of the application's container. Supported event types are types which extend CompositePresentationEvent. <para/>
    /// 3) AutoInvalidateOnSelection - RaiseCanExecuteChanged() is automatically called when the selection of the specified type (resolved from the 
    ///                                event aggregator instance of the application's container) changes. Supported selection types are types which extend SelectionBase.
    /// </summary>
    public class SubCommand : StaticMetadataOwnerCommand<SubCommandMetadataCollection, ISubCommandMetadata>, ISubCommand
    {
    }
}

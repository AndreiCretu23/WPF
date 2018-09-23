using Quantum.Metadata;

namespace Quantum.Command
{
    /// <summary>
    /// Provides the basic contract for a parameterless delegate command.
    /// </summary>
    public interface IDelegateCommand : IStaticCommand
    {
    }

    /// <summary>
    /// Provides the basic contract for a simple delegate command that takes one parameter of a certain type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDelegateCommand<T> : IDependencyCommand
    {
    }

    /// <summary>
    /// Provides the basic contract for a managed command.<para/>
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
    public interface IManagedCommand : IStaticMetadataOwnerCommand<CommandMetadataCollection, ICommandMetadata>
    {
    }


    /// <summary>
    /// Provides the basic contract for a multi managed command. <para/>
    /// Multi managed commands represent a collection of application-wide commands provided by a getter delegate which are processed by the 
    /// main window of the application depending on their individual metadata. They must be defined as properties in command containers registered 
    /// in the command manager service of the framework. After they get registered in the command manager service cache, they are processed by the services 
    /// which create the main window of the application depending on their metadata : <para/>
    /// 1) MultiMainMenuOption - The set of commands returned by the getter delegate will appear as entries in the main menu of the application(menu location
    ///                          and other factors are configurable in each subcommand's metadata inside the getter delegate).
    ///                          This metadata type does not support multiple instances in the metadata collection of a multi managed command. <para/>
    /// 2) AutoInvalidateOnEvent - Instructs the framework to re-evaluate the commands when the specified event type is fired in the event aggregator instance 
    ///                            of the application's container. The current set of commmands associated with this MultiManagedCommand will be wiped out, 
    ///                            and the getter delegate will be used to re-compute the commands. After that, the new commands will be processed by the framework 
    ///                            depending on their individual metadata. Supported event types are types which extend CompositePresentationEvent. <para/>
    /// 3) AutoInvalidateOnSelection - Instructs the framework to re-evaluate the commands when the selection of the specified type (resolved from the 
    ///                                event aggregator instance of the application's container) changes. The current set of commmands associated with 
    ///                                this MultiManagedCommand will be wiped out, and the getter delegate will be used to re-compute the commands. 
    ///                                After that, the new commands will be processed by the framework depending on their individual metadata.
    ///                                Supported selection types are types which extend SelectionBase. <para/>
    /// </summary>
    public interface IMultiManagedCommand : IMultiStaticMetadataOwnerCommand<ISubCommand, MultiCommandMetadataCollection, IMultiCommandMetadata>
    {
    }


    /// <summary>
    /// Provides the basic contract for a subcommand.<para/>
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
    public interface ISubCommand : IStaticMetadataOwnerCommand<SubCommandMetadataCollection, ISubCommandMetadata>
    {
    }
}

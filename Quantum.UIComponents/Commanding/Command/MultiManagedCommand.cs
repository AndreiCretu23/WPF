using Quantum.Metadata;

namespace Quantum.Command
{
    /// <summary>
    /// Multi managed commands represent a collection of application-wide commands provided by a getter delegate which are processed by the 
    /// main window of the application depending on their individual metadata. They must be defined as properties in command containers registered 
    /// in the command manager service of the framework. After they get registered in the command manager service cache, they are processed by the services 
    /// which create the main window of the application depending on their metadata : <para/>
    /// 1) MultiMainMenuOption - The set of commands returned by the getter delegate will appear as entries in the main menu of the application(menu location
    ///                          and other factors are configurable in each subcommand's metadata inside the getter delegate).
    ///                          This metadata type does not support multiple instances in the metadata collection of a multi managed command. <para/>
    /// 2) AutoInvalidateOnEvent - Instructs the framework to re-evaluate the commands when the specified event type is fired in the event aggregator instance 
    ///                            of the application's container. The current set of commmands associated with this MultiManagedCommand will be wiped out, 
    ///                            and the getter delegate will be used to re-compute the commands. After that, the new commands will be re-processed by the framework 
    ///                            depending on their individual metadata. Supported event types are types which extend CompositePresentationEvent. <para/>
    /// 3) AutoInvalidateOnSelection - Instructs the framework to re-evaluate the commands when the selection of the specified type (resolved from the 
    ///                                event aggregator instance of the application's container) changes. The current set of commmands associated with 
    ///                                this MultiManagedCommand will be wiped out, and the getter delegate will be used to re-compute the commands. 
    ///                                After that, the new commands will be re-processed by the framework depending on their individual metadata.
    ///                                Supported selection types are types which extend SelectionBase. <para/>
    /// </summary>
    public class MultiManagedCommand : MultiStaticMetadataOwnerCommand<ISubCommand, MultiCommandMetadataCollection, IMultiCommandMetadata>, IMultiManagedCommand
    {
    }
}

namespace Quantum.Metadata
{
    /// <summary>
    /// A metadata type used as a unique identifier for a command. The framework uses this information in order 
    /// to match various deserialized data with their associated commands.
    /// </summary>
    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class CommandGuid : ICommandMetadata
    {
        public string Guid { get; }

        public CommandGuid(string guid)
        {
            Guid = guid;
        }
    }
}

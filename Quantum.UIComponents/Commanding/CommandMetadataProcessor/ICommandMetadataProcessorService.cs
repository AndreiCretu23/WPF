namespace Quantum.Command
{
    internal interface ICommandMetadataProcessorService
    {
        void ProcessMetadata(IManagedCommand command);
    }
}

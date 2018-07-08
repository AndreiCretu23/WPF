namespace Quantum.Command
{
    internal interface ICommandMetadataProcessorService
    {
        void ProcessMetadata(ICommandBase command);
    }
}

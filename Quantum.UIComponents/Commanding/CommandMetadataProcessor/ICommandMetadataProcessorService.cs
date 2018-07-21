namespace Quantum.Command
{
    public interface ICommandMetadataProcessorService
    {
        void ProcessMetadata(ICommandBase command);
    }
}

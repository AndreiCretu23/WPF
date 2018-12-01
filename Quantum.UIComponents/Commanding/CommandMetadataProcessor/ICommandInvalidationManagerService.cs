namespace Quantum.Command
{
    internal interface ICommandInvalidationManagerService
    {
        void ProcessInvalidators(object command);
    }
}

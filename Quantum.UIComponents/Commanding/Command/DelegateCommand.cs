namespace Quantum.Command
{
    /// <summary>
    /// Provides a simple parameterless command, with settable CanExecute/Execute delegates.
    /// </summary>
    public class DelegateCommand : StaticCommand, IDelegateCommand
    {
    }

    /// <summary>
    /// Provides a simple command that takes one parameter of a certain type with settable CanExecute/Execute delegates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelegateCommand<T> : DependencyCommand<T>, IDelegateCommand<T>
    {
    }
}

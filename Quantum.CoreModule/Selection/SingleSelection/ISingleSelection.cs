namespace Quantum.Services
{
    public interface ISingleSelection : ISelection
    {
        ISingleSelectionCache OldValue { get; }
    }
}

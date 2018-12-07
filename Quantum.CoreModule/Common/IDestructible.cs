namespace Quantum.Common
{
    /// <summary>
    /// This interface is implemented by objects that represent a destructibe object. Various framework components will call the TearDown method
    /// of objects that implement this interface when the objects are disposed from usage. Example : DialogViewModels, DynamicPanelViewModels, etc
    /// </summary>
    public interface IDestructible
    {
        void TearDown();
    }
}

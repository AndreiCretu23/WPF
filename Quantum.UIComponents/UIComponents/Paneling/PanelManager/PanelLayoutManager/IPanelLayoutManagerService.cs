namespace Quantum.UIComponents
{
    internal interface IPanelLayoutManagerService
    {
        bool LoadLayout(string directory);
        bool SaveLayout(string directory);
    }
}

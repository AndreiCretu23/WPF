namespace Quantum.UIComponents
{
    internal interface IPanelLayoutManagerService
    {
        bool LoadLayout(string layoutFileName);
        bool SaveLayout(string layoutFileName);
    }
}

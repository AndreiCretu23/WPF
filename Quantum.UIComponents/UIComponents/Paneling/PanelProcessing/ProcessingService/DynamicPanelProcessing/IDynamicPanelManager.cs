namespace Quantum.UIComponents
{
    internal interface IDynamicPanelManager
    {
        IDynamicPanelDefinition Definition { get; }

        void ProcessDefinition();
        void BringPanelIntoView(object viewModel);
    }
}

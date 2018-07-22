namespace Quantum.Metadata
{
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class ToolTip : IMenuMetadata, ISubMenuMetadata
    {
        public ToolTip(string toolTip)
        {
            Value = toolTip;
        }
        
        public string Value { get; private set; }
    }
}

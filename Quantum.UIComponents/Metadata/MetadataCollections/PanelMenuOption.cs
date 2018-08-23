namespace Quantum.Metadata
{
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class PanelMenuOption : MetadataCollection<IPanelMenuEntryMetadata>, IStaticPanelMetadata
    {
    }
}

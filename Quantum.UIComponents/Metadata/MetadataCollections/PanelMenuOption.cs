namespace Quantum.Metadata
{
    [Mandatory(false)]
    [SupportsMultiple(false)]
    [MandatoryCollection(false)]
    public class PanelMenuOption : MetadataCollection<IPanelMenuEntryMetadata>, IStaticPanelMetadata
    {
    }
}

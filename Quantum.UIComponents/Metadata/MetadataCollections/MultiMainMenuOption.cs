namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type is used for requesting a set of entries into the main menu of the application.
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// The location of the entries is defined by a MenuPath metadata type that must be included in any 
    /// MultiMainMenuOption. The appearence of the entries are defined individually for each entry.
    /// </summary>
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class MultiMainMenuOption : MetadataCollection<IMultiMainMenuMetadata>, IMultiCommandMetadata
    {
    }
}

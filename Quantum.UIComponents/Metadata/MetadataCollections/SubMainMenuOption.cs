namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type is used for requesting an entry into the main menu of the application for a
    /// component of a parent set which owns a MultiMainMenuOption metadata.
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// The location of the menu entry depends on the owner's MultiMainMenuOption's metadata and the 
    /// order of the component in the parent set. The appearence in the main menu of the application depends 
    /// on the SubMainMenuOption's own metadata : <para/>
    /// 1) Description -> This metadata type indicates the new menu entry's header. WARNING : Any SubMainMenuOption metadata collection MUST have exactly one Description. <para/>
    /// 2) Icon -> This metadata type indicates the new menu entry's icon. A SubMainMenuOption can have maximum one metadata of this type. <para/>
    /// 3) ToolTip -> This metadata type indicates the new menu entry's tooltip. A SubMainMenuOption can have maximum one metadata of this type. <para/>
    /// 4) Checkable -> This metadata type indicates if the new menu entry is checkable or not. If the parent SubMainMenuOption collection does not contain this metadata type, the default value will be set to false. <para/>
    /// 5) CheckChanged -> This metadata type provides an action that is to be executed when the check state of the menu entry changes. Only makes sense if the parent SubMainMenuOption contains a Checkable(true) metadata. <para/>
    /// </summary>
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class SubMainMenuOption : MetadataCollection<ISubMainMenuMetadata>, ISubCommandMetadata
    {
    }
}
